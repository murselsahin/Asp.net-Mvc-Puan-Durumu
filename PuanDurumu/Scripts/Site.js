$("#EvSahibi_Id").change(function () {
    
    var evSahibi_Id = $(this).val();

    if (evSahibi_Id != "" || evSahibi_Id != "-1") {

        $("#Deplasman_Id option").not(":first").remove();

        $.ajax({
            method: "GET",
            url: '/Home/GetDeplasmanTakimi' + '/' + evSahibi_Id
        }).done(function (result) {
            var ddlDeplasman = $("#Deplasman_Id");
            for (var i = 0; i < result.length; i++) {
                var adi = result[i].Adi;
                var id = result[i].Id;

                var opt = $("<option></option>");

                opt.text(adi);
                opt.val(id);

                ddlDeplasman.append(opt);

            }
        })


    } else {

        $("#Deplasman_Id option").not(":first").remove();
    }
});

$("#skorGirForm").submit(function (e) {
    if ($.trim($("#Maclar_EvSkor").val()) == "" || $.trim($("#Maclar_DeplasmanSkor").val()) == "") {

        e.preventDefault();
        alert("Skorlar Boş Girilemez");
        return;
    }

    if ($.trim($("#EvSahibi_Id").val()) == "" || $.trim($("#Deplasman_Id").val()) == "") {

        e.preventDefault();
        alert("Takımları Seçmeniz Lazım");
    }
});






