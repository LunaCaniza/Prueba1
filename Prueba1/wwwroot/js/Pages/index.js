let table;

Date.prototype.toDateInputValue = (function () {
    var local = new Date(this);
    local.setMinutes(this.getMinutes() - this.getTimezoneOffset());
    return local.toJSON().slice(0, 10);
});

function DefaultSelectsFill(url, selects) {
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        success: function (json) {
            if (json.success) {

                selects.forEach(function (value) {
                    json.data[value].forEach(function (item) {
                        $("#select_" + value).append("<option value='" + item.value + "'>" + item.text + "</option>");
                    });
                });

            }
            else {
                toastr.error("Erro ao popular selects:" + data.message);
            }
        },
        error: function (request, error) {
            toastr.error("Erro:" + JSON.stringify(request));
        }
    });
}
function DefaultDTButtons(urlNovo, pdfMessage) {
    return [
        {
            extend: 'excel',
            text: '<i class="fa fa-file-excel"></i>&nbspDownload',
            className: 'btn btn-sm btn-success  mb-2',
            messageTop: pdfMessage,
            action: newExportAction,
            title: 'Colegio Crecer - Servus'
        },
        {
            extend: 'pdfHtml5',
            text: '<i class="fa fa-file-pdf"></i>&nbspPDF',
            className: 'btn btn-sm btn-danger mb-2',
            messageTop: pdfMessage,
            action: newExportAction,
            title: 'Colegio Crecer - Servus'
        },
        {
            extend: 'print',
            text: '<i class="fa fa-print"></i>&nbspImprimir',
            className: 'btn btn-sm btn-primary  mb-2',
            messageTop: pdfMessage,
            action: newExportAction,
            title: 'Colegio Crecer - Servus'
        },
        {
            extend: 'copy',
            text: '<i class="fa fa-copy"></i>&nbspCopiar',
            className: 'btn btn-sm btn-info mb-2',
            action: newExportAction
        },
        {            
            text: 'Nuevo',
            className: 'btn btn-sm btn-success mb-2 float-right',
            action: function (e, dt, node, config) {
                window.location.href = urlNovo;
            }            
        }
    ];
}

function DefaultDateEvent() {
    $(".DateRangeInput").on('change', function () {
        table.draw();
    });
}

function DefaultSelectEvent() {

    $(".SearchFormInput").on('change', function () {
        let select = $(event.target);
        table.columns(select.attr('dtCol')).search(select.val()).draw();
    });
}

var oldExportAction = function (self, e, dt, button, config) {

    if (button[0].className.indexOf('buttons-excel') >= 0) {
        if ($.fn.dataTable.ext.buttons.excelHtml5.available(dt, config)) {
            $.fn.dataTable.ext.buttons.excelHtml5.action.call(self, e, dt, button, config);
        }
        else {
            $.fn.dataTable.ext.buttons.excelFlash.action.call(self, e, dt, button, config);
        }
    } else if (button[0].className.indexOf('buttons-print') >= 0) {
        $.fn.dataTable.ext.buttons.print.action(e, dt, button, config);
    } else if (button[0].className.indexOf('buttons-pdf') >= 0) {
        $.fn.dataTable.ext.buttons.pdfHtml5.action.call(self, e, dt, button, config);
    } else {
        $.fn.dataTable.ext.buttons.copyHtml5.action.call(self, e, dt, button, config);
        toastr.info("Copiado para área de transferência");
    }
};
var newExportAction = function (e, dt, button, config) {
    var self = this;
    var oldStart = dt.settings()[0]._iDisplayStart;

    dt.one('preXhr', function (e, s, data) {
        // Just this once, load all data from the server...
        data.start = 0;
        data.length = 2147483647;

        dt.one('preDraw', function (e, settings) {
            // Call the original action function 
            oldExportAction(self, e, dt, button, config);

            dt.one('preXhr', function (e, s, data) {
                // DataTables thinks the first item displayed is index 0, but we're not drawing that.
                // Set the property to what it was before exporting.
                settings._iDisplayStart = oldStart;
                data.start = oldStart;
            });

            // Reload the grid with the original page. Otherwise, API functions like table.cell(this) don't work properly.
            setTimeout(dt.ajax.reload, 0);

            // Prevent rendering of the full data to the DOM
            return false;
        });
    });

    // Requery the server with the new one-time export settings
    dt.ajax.reload();
};

function DeletarItemAjax(controller, id) {

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };

    $.ajax({
        url: '/' + controller + '/Delete/' + id,
        type: 'DELETE',
        dataType: 'json',
        success: function (data) {
            if (data.success) {
                toastr.success("Eliminado con éxito.");
                table.draw();
            }
            else {
                toastr.error("Erro:" + data.message);
            }
        },
        error: function (request, error) {
            toastr.error("Erro:" + JSON.stringify(request));
        }
    });
}


function ConfirmDelete(controller, id, message = "¿Estas seguro que quieres borrarlo?") {
    $.getScript("https://cdnjs.cloudflare.com/ajax/libs/bootbox.js/5.5.2/bootbox.min.js", function (data, textStatus, jqxhr) {
        bootbox.confirm({
            message: message,
            buttons: {
                confirm: {
                    label: 'Sí',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                if (result) {
                    DeletarItemAjax(controller, id);
                }
            }
        });
    });
}

function DTActDet(controller, id) {
    return '<a class="btn btn-xs btn-outline-secondary rounded" href="/' + controller + '/Details/' + id + '"><i class="fa fa-eye"></i></a>';
}
function DTActEdit(controller, id) {
    return '<a class="btn btn-xs btn-outline-secondary ml-1 rounded" href="/' + controller + '/Edit/' + id + '"><i class="fa fa-edit"></i></a>';
}
function DTActDel(controller, id) {
    return '<button class="btn btn-xs btn-outline-secondary ml-1 rounded" onClick="ConfirmDelete(\'' + controller + '\',' + id + ')"><i class="fa fa-trash"></i></button>';
}
function RenderTableActions(controller, id, details = true, edit = true, del = true) {
    let output;
    output = "";

    if (details)
        output += DTActDet(controller, id);

    if (edit)
        output += DTActEdit(controller, id);

    if (del)
        output += DTActDel(controller, id);

    return output;
}
