function DefaultItemLocalEvent() {
    $(".DestinoSelect").selectpicker('hide');
    $("#Destino1").selectpicker('show');

    $("#LocalSelect").on('change', function (event) {
        let val = $(event.target).val();
        $(".DestinoSelect").selectpicker('hide');
        $(".DestinoSelect").selectpicker('val', "");
        $("#Destino" + val).selectpicker('show');
    });

}
