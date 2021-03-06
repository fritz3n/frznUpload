﻿using System;
using System.Collections.Generic;
using System.Text;

namespace frznUpload.Shared
{
	class MessagePatterns
	{
		private class Types
		{
			public const int String = 1;
			public const int Int = 2;
			public const int Raw = 4;
			public const int Message = 8;
		}

		private class Control
		{
			public const int Indexer = 16;
			public const int Optional = 32;
			public const int Idk = 64;
		}

		static readonly Dictionary<Message.MessageType, List<int>> patterns = new Dictionary<Message.MessageType, List<int>>
		{
			{Message.MessageType.Ping, new List<int>{ Types.Int, Types.Int} },
			{Message.MessageType.Pong, new List<int>{ Types.Int, Types.Int} },

			{Message.MessageType.CertRevokeRequest, new List<int>() },
			{Message.MessageType.CertRevokeSuccess, new List<int>() },

			{Message.MessageType.AuthSuccess, new List<int>{ Types.String } }, // username

			{Message.MessageType.CertRequest, new List<int>{Types.String, Types.String, Types.Raw, Types.Raw, Types.String} }, // username, password, pubKey
            {Message.MessageType.CertSuccess, new List<int>{ Types.Raw } }, // certified certificate

			{Message.MessageType.CertRenewRequest, new List<int>{ Types.Raw, Types.Raw, Types.String } }, // pubKey
            {Message.MessageType.CertRenewSuccess, new List<int>{ Types.Raw } }, // certified certificate

			{Message.MessageType.TwoFactorNeeded, new List<int>{ Types.String} },
			{Message.MessageType.TwoFactorAdd, new List<int>{ Types.String} },
			{Message.MessageType.TwoFactorRemove, new List<int>{ } },
			{Message.MessageType.TwoFactorSuccess, new List<int>{ } },
			{Message.MessageType.HasTwoFactor, new List<int>{ Types.Int } },

			{Message.MessageType.FileUploadRequest, new List<int>{ Types.String, Types.String, Types.String, Types.Int } },
			{Message.MessageType.FileUploadApproved, new List<int>{ Types.String } },
			{Message.MessageType.FileUpload, new List<int>{ Types.Int, Types.Raw } },
			{Message.MessageType.FileUploadFinished, new List<int>{} },
			{Message.MessageType.FileUploadSuccess, new List<int>{} },
            //                                              file_identifier, First_view,Public,Public_registered,Whitelisted,Whitelist
            {Message.MessageType.ShareRequest, new List<int>{ Types.String, Types.Int, Types.Int, Types.Int, Types.Int, Types.String} },
			{Message.MessageType.ShareResponse, new List<int>{ Types.String } },

			{Message.MessageType.FileListRequest, new List<int>() },
			{Message.MessageType.FileList, new List<int>{ Types.Int | Control.Indexer ,  Types.Message | (int)Message.MessageType.FileInfo << 4} },
			{Message.MessageType.FileInfo, new List<int>{ Types.String, Types.String, Types.String, Types.Int, Types.String} },

			{Message.MessageType.ShareListRequest, new List<int>( Types.String ) },
			{Message.MessageType.ShareList, new List<int>{ Types.String, Types.Int | Control.Indexer ,  Types.Message | (int)Message.MessageType.ShareInfo << 4} },

            //Delete Share                                    file_identifier
            {Message.MessageType.DeleteFile, new List<int> { Types.String } },

            //                                                        file_identifier, First_view,Public,Public_registered,Whitelisted,Whitelist
            {Message.MessageType.ShareInfo, new List<int>{Types.String, Types.String, Types.Int, Types.Int, Types.Int, Types.Int, Types.String} },

			{Message.MessageType.Version, new List<int>{Types.Int} },

			{Message.MessageType.Sequence, new List<int>{ Control.Idk } },
			{Message.MessageType.None, new List<int>{ Control.Idk } },
		};

		public static (bool, string) CheckMessage(Message m)
		{
			if (m.IsError)
				return (true, "Message is error");

			List<int> fields = patterns[m.Type];

			bool optional = false;

			int pi = 0;

			for (int i = 0; i < m.Count; i++)
			{
				if (pi > fields.Count - 1)
					return (true, "There are too many Message fields");

				int curField = fields[pi];
				int options = curField & 0xf0;

				switch (options)
				{
					case Control.Indexer:

						if (m.FieldTypes[i] != Message.FieldType.Int)
							return (false, $"Field {i} is marked as an integer but its type is {m.FieldTypes[i]}");
						i++;

						pi++;
						for (int j = i + m[i - 1]; i < j; i++)
						{
							if (!TypesMatch(m.FieldTypes[i], fields[pi], m[i]))
								return (false, $"Field {i}: {m.FieldTypes[i]} doesn´t match {curField}");
						}

						pi++;

						break;

					case Control.Idk:
						return (true, null);

					case Control.Optional:
						optional = true;
						goto default;

					default:
						if (!TypesMatch(m.FieldTypes[i], curField, m[i]))
							return (false, $"Field {i}: {m.FieldTypes[i]} doesn´t match {curField}");
						pi++;
						break;
				}

			}

			if (!optional & pi < fields.Count - 1)
			{
				if ((fields[pi + 1] & Control.Optional) > 0)
					return (true, null);

				return (false, "There are to few Message fields!");
			}

			return (true, null);
		}

		private static bool TypesMatch(Message.FieldType fieldType, int intType, object field = null)
		{
			int intTypeNoCrap = intType & 0xf;
			int convertedMessageType = (int)Math.Pow(2, (int)fieldType);

			if (intTypeNoCrap != convertedMessageType)
				return false;

			if ((intType & Types.Message) > 0)
			{
				int embeddedIntType = intType >> 4;
				if ((int)(field as Message).Type != embeddedIntType)
					return false;
			}

			return true;
		}

	}
}
