$(document).ready(function () {

});
function enviarValSelect(idi, idf, attrf) {
    $("#" + idf).val($("#" + idi + " option:selected").attr(attrf));
}
function enviarValList(idi, idList, idf, attrf) {
    $("#" + idf).val($("#" + idList + " option[value='" + $("#" + idi).val() + "']").attr(attrf));
}