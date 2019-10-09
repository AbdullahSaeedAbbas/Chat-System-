// JavaScript source code
function ChangeBorder(x) {
    document.getElementById(x).style.transition = "0.5s";
    if (document.getElementById(x).style.borderRadius == "25px") {
        document.getElementById(x).style.borderRadius = "5px"
    }
    else {
        document.getElementById(x).style.borderRadius = "25px"
    }
}
//document.getElementById("body_main").style.width = window.innerWidth ;
//document.getElementById("body_main").style.height = window.innerHeight - 100;

//var x = "Total Width: " + screen.width + "px";
//document.getElementById("demo").innerHTML = x;


//div.style.mozTransform = 'rotate(' + deg + 'deg)'; 
function RegisterForm() {

    var xx = document.getElementById("main");
    xx.style.transition = '0.2s';
    xx.style.mozTransform = 'rotateY(180deg)';
    xx.style.webkitTransform = 'rotateY(180deg)';
    xx.div.style.msTransform = 'rotateY(180deg)';
}
function RegisterForm2() {

    var xx = document.getElementById("main");
    xx.style.transition = '0.6s';
    xx.style.mozTransform = 'rotateY(-0deg)';
    xx.style.webkitTransform = 'rotateY(-0deg)';
    xx.div.style.msTransform = 'rotateY(-0deg)';
}
//function aass() {
//    var w = $(window).width();
//    var h = $(window).height();
//    $('#main').css('width', w);
//    //$('.Login_form').css('height', h);
//}
function ee() {
    var w = $(window).width();
    var h = $(window).height();
    $('#MainDiv').css('width', w - 20);
    $('#MainDiv').css('height', h -62);

}
function ChangeColorBtnOnClicked(Btn)
{
    var btnName = "MyAccountBtn";
    var count = document.getElementsByTagName("table").length;
    for (var i = 0; i < 9; i++) {
        if (Btn == btnName + i) {
            document.getElementById(Btn).style.backgroundColor = "green";
            document.getElementById(Btn).style.color = "black";
            document.getElementById(Btn).style.borderRightWidth = "0px";
            document.getElementById(Btn).style.borderRightStyle = "none";
        }
        else {
            document.getElementById(Btn).style.backgroundColor = "rgba(0, 0, 0, 0.5)";
            document.getElementById(Btn).style.color = "white";

        }
    } 
   
  
}

