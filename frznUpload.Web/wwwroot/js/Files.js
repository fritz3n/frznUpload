
function sizeSort(a, b, rowA, rowB) {
    if (a == "" && b != "")
        return -1;
    if (b == "" && a != "")
        return 1;

    let sizeA = rowA["_size_data"]["bytes"];
    let sizeB = rowB["_size_data"]["bytes"];

    if (sizeA > sizeB)
        return 1;
    if (sizeA < sizeB) 
        return -1;
    return 0;
}
function shareSort(a, b, rowA, rowB) {
    if (a == "" && b != "")
        return -1;
    if (b == "" && a != "")
        return 1;

    let sizeA = rowA["_shares_data"]["count"];
    let sizeB = rowB["_shares_data"]["count"];

    if (sizeA > sizeB)
        return 1;
    if (sizeA < sizeB)
        return -1;
    return 0;
}

$("table").bootstrapTable({
    pagination: true,
    search: true,
    showPaginationSwitch: true,
    pageSize: 15,
    pageList: [15, 35, 50, 100, "all"],
    classes: 'table table-hover',
    columns: [
        {
            field: 'name',
            title: 'Filename'
        },
        {
            field: 'size',
            title: 'Size',
            sorter: sizeSort,
            order: "desc"
        },
        {
            field: 'id',
            title: 'Identifier',
            searchable: false,
            sortable: false
        },
        {
            field: 'shares',
            title: 'Shares',
            sorter: shareSort
        },
    ]
});


$("table").on("click-row.bs.table", function (row, $element, field) {
    if ($element["_data"]["path"]) {
        window.location = "/Account/Files/Index" + $element["_data"]["path"];
    } else if ($element["_data"]["identifier"]) {
        window.location = "/Account/Files/View/" + $element["_data"]["identifier"];
    }
});
