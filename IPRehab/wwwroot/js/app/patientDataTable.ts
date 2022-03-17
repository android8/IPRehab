/// <reference path="../../node_modules/@types/jquery/jquery.d.ts" />

$(function () {
  let url: string = '';
  if (window.location.host.indexOf('localhost') == -1
    //|| $(location).attr('href').indexOf('localhost') == -1
  ) {
    //not localhost
    url = '/iprehabmetrics/' + url;
  }
  else {
    url = url + '/';
  }

  const groupColumn: number = 1; //group by column 2 (column index 1)
  const targetTable: any = $('table.patients');

  //dom options
  const uploadButtonStyle: string = '<"btn primary #u">';
  const downloadButtonStyle: string = '<"btn primary #d">';
  //built-in table control elements 
  //l - length changing input control
  //f - filtering input
  //t - The table!
  //i - Table information summary
  //p - pagination control
  //r - processing display element
  const navArrangement: string =
    '<"row"<"col-md-2 col-sm-3"l>' +
    '<"col-md-8 col-sm-6"' + uploadButtonStyle + downloadButtonStyle + 'p> ' +
    '<"col-md-2 col-sm-3"f>>' + 'rt' +
    '<"row"<"col-md-2 col-sm-2"i>' +
    '<"col-md-8 col-sm-8"p>' +
    '<"col-md-2 col-sm-2"f>>';
  const navButtons: Array<object> = [
    {
      text: '<i class="fa fa-files-o"></i>',
      titleAttr: 'Copy'
    },
    {
      text: '<i class="fa fa-file-excel-o"></i>',
      titleAttr: 'Excel'
    },
    {
      text: '<i class="fa fa-file-text-o"></i>',
      titleAttr: 'CSV'
    },
    {
      text: '<i class="fa fa-file-pdf-o"></i>',
      titleAttr: 'PDF'
    }
  ];
  const navButtons2: string[] = ['copy', 'csv', 'excel', 'pdf'];

  let collapsedGroups: {} = {};
  targetTable.DataTable();
  //targetTable.DataTable({
  //  "fixedHeader": true,
  //  "autowidth": true,
  //  //"dom": "lBfrtip",
  //  //"buttons": navButtons2,
  //  "responsive": true,
  //  "columnDefs": [
  //    { "visible": true, "targets": groupColumn }
  //  ],
  //  "order": [[groupColumn, 'desc']],
  //  stateSave: true,
  //  stateSaveCallback: function (settings, data) {
  //    localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data))
  //  },
  //  stateLoadCallback: function (settings) {
  //    return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance))
  //  },
  //  "pageLength": 25,
  //  "pagingType": "full_numbers",
  //  "drawCallback": function (settings) {
  //    var api = this.api();
  //    var rows = api.rows({ page: 'current' }).nodes();
  //    var last = null;
  //    targetTable.show();

  //    //api.column(groupColumn, { page: 'current' }).data().each(function (group, i) {
  //    //  if (last !== group) {
  //    //    $(rows).eq(i).before(
  //    //      //add download all and print all icons
  //    //      '<tr class="group">' +
  //    //      '<td colspan="5">' + group +
  //    //      '<a href="' + url + 'UserAnswer/ExportExcel?fy=' + group + '" class="exportAll" title ="Download ' + group + ' data for ALL sites" onclick="$(\'.spinnerContainer\').show()" data-fy="' + group + '">' +
  //    //      '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-graph-down" viewBox="0 0 16 16">' +
  //    //      '<path fill-rule="evenodd" d="M0 0h1v15h15v1H0V0zm10 11.5a.5.5 0 0 0 .5.5h4a.5.5 0 0 0 .5-.5v-4a.5.5 0 0 0-1 0v2.6l-3.613-4.417a.5.5 0 0 0-.74-.037L7.06 8.233 3.404 3.206a.5.5 0 0 0-.808.588l4 5.5a.5.5 0 0 0 .758.06l2.609-2.61L13.445 11H10.5a.5.5 0 0 0-.5.5z" />' +
  //    //      '</svg>' +
  //    //      '</a>' +
  //    //      '<a href=' + url + '"UserAnswer/Print?fy=' + group + '" class="printAll hidden" title="Print ' + group + ' data for All sites" onclick="$(\'.spinnerContainer\').show()" data-fy="' + group + '">' +
  //    //          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-printer" viewBox="0 0 16 16">' +
  //    //            '<path d="M2.5 8a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1z" />'+
  //    //            '<path d="M5 1a2 2 0 0 0-2 2v2H2a2 2 0 0 0-2 2v3a2 2 0 0 0 2 2h1v1a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2v-1h1a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-1V3a2 2 0 0 0-2-2H5zM4 3a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v2H4V3zm1 5a2 2 0 0 0-2 2v1H2a1 1 0 0 1-1-1V7a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v3a1 1 0 0 1-1 1h-1v-1a2 2 0 0 0-2-2H5zm7 2v3a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1v-3a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1z" />' +
  //    //          '</svg>' +
  //    //        '</a>' +
  //    //      '</td>' +
  //    //      '</tr>'
  //    //    );
  //    //    last = group;
  //    //  }
  //    //});
  //  },
  //  "rowGroup": {
  //    // Uses the 'row group' plugin
  //    dataSrc: groupColumn,
  //    startRender: function (rows, group) {
  //      var collapsed = !!collapsedGroups[group];

  //      rows.nodes().each(function (r) {
  //        r.style.display = collapsed ? 'none' : '';
  //      });

  //      // Add category name to the <tr>. NOTE: Hardcoded colspan
  //      return $('<tr/>')
  //        .append('<td colspan="5">' + group + ' (' + rows.count() + ')' +
  //          '<a href="' + url + 'Patient/ExportExcel?fy=' + group + '" class="exportAll" title ="Download ' + group + ' data for ALL sites" data-fy="' + group + '" onclick="$(\'.spinnerContainer\').show(); alert(\'It may take a while to download, please click the progress bars to dismiss them when done.\')">' +
  //          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-graph-down" viewBox="0 0 16 16">' +
  //          '<path fill-rule="evenodd" d="M0 0h1v15h15v1H0V0zm10 11.5a.5.5 0 0 0 .5.5h4a.5.5 0 0 0 .5-.5v-4a.5.5 0 0 0-1 0v2.6l-3.613-4.417a.5.5 0 0 0-.74-.037L7.06 8.233 3.404 3.206a.5.5 0 0 0-.808.588l4 5.5a.5.5 0 0 0 .758.06l2.609-2.61L13.445 11H10.5a.5.5 0 0 0-.5.5z" />' +
  //          '</svg>' +
  //          '</a>' +
  //          '<a href=' + url + '"Patient/Print?fy=' + group + '" class="printAll hidden" title="Print ' + group + ' data for All sites" onclick="$(\'.spinnerContainer\').show()" data-fy="' + group + '">' +
  //          '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-printer" viewBox="0 0 16 16">' +
  //          '<path d="M2.5 8a.5.5 0 1 0 0-1 .5.5 0 0 0 0 1z" />' +
  //          '<path d="M5 1a2 2 0 0 0-2 2v2H2a2 2 0 0 0-2 2v3a2 2 0 0 0 2 2h1v1a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2v-1h1a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-1V3a2 2 0 0 0-2-2H5zM4 3a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1v2H4V3zm1 5a2 2 0 0 0-2 2v1H2a1 1 0 0 1-1-1V7a1 1 0 0 1 1-1h12a1 1 0 0 1 1 1v3a1 1 0 0 1-1 1h-1v-1a2 2 0 0 0-2-2H5zm7 2v3a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1v-3a1 1 0 0 1 1-1h6a1 1 0 0 1 1 1z" />' +
  //          '</svg>' +
  //          '</a>' +
  //          '</td>')
  //        .attr('data-name', group)
  //        .toggleClass('collapsed', collapsed);
  //    }
  //  }
  //});
  //the server render html table with display:none to hide the initial html table.  show() to unhide it after datatable convertion is completed. 

  $('tbody', targetTable).on('click', 'tr.dtrg-start', function () {
    var name = $(this).data('name');
    console.log('group name', name);
    collapsedGroups[name] = !collapsedGroups[name];
    console.log('collapsedGroups[' + name + ']', collapsedGroups[name]);
    //targetTable.destroy();
    //targetTable.draw();
  });

  //$('tbody', targetTable).on('click', 'tr.dtrg-group', function () {
  //  var currentOrder = targetTable.order();
  //  console.log("dataTable current order", currentOrder);

  //  if (currentOrder[0] === groupColumn && currentOrder[1] === 'asc') {
  //    targetTable.order([groupColumn, 'desc']).draw();
  //  }
  //  else {
  //    targetTable.order([groupColumn, 'asc']).draw();
  //  }
  //});

  //export datatable as CSV using ajax

  //export datatable all rows using ajax
});
