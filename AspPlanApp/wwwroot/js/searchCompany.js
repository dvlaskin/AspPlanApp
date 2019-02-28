$(document).ready(function() {

    $("#js-live-search").keyup(function() {

        var name = $('#js-live-search').val();

        console.log(name);

        if (name == "") {
            $("#js-display").hide();
        } else {
            $.ajax({
                type: "GET",
                url: "api/SearchOrgByName",
                data: {
                    strOrg: name
                },

                success: function(resultOrg) {

                    var resultHtml = "";
                    resultOrg.forEach(function(item) {

                        resultHtml +=
                            `<li id="js-found-org" 
                                class="found-item"
                                orgId="${item.orgId}" 
                                orgName="${item.orgName}">${item.orgName} - ${item.orgInfo}</li>`;
                    });

                    //console.log(resultHtml);

                    if (resultHtml) {
                        $("#js-display").show();
                        $("#js-found-org").html(resultHtml);
                        $("#js-found-org").show();
                    } else {
                        $("#js-display").hide();
                    }
                }
            });
        }
    });

    $(window).click(function() {
        $("#js-display").hide();
    });

    $(document).on('click','.found-item',function(e) {

        //console.log("click");

        e.preventDefault();

        var currObj = $(this);
        var orgId = currObj.attr("orgId");
        var orgName = currObj.attr("orgName");

        $("#js-orgId").val(orgId);
        $("#js-live-search").val(orgName);
        $("#js-display").hide();

    });

});