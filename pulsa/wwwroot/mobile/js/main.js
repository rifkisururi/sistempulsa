const username = document.getElementById("kodeVoucher")
const password = document.getElementById("password")
function setpass() {
  var user = username.value
  password.value = user;
}
if(username !== null)
{
  username.onchange = setpass;
}
// Dark Mode
var themeToggleDarkIcon = document.getElementById('theme-toggle-dark-icon');
var themeToggleLightIcon = document.getElementById('theme-toggle-light-icon');

// Change the icons inside the button based on previous settings
if (localStorage.getItem('color-theme') === 'dark' || (!('color-theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
    
    document.documentElement.classList.add('dark');
    themeToggleLightIcon.classList.remove('hidden');
} else {
    
    document.documentElement.classList.remove('dark');
    themeToggleDarkIcon.classList.remove('hidden');
}

var themeToggleBtn = document.getElementById('toggle-theme');

themeToggleBtn.addEventListener('click', function() {

    // toggle icons inside button
    themeToggleDarkIcon.classList.toggle('hidden');
    themeToggleLightIcon.classList.toggle('hidden');
    
    
    // if set via local storage previously
    if (localStorage.getItem('color-theme')) {
        if (localStorage.getItem('color-theme') === 'light') {
            document.documentElement.classList.add('dark');
            localStorage.setItem('color-theme', 'dark');
        } else {
            document.documentElement.classList.remove('dark');
            localStorage.setItem('color-theme', 'light');
        }
    
    // if NOT set via local storage previously
    } else {
        if (document.documentElement.classList.contains('dark')) {
            document.documentElement.classList.remove('dark');
            localStorage.setItem('color-theme', 'light');
        } else {
            document.documentElement.classList.add('dark');
            localStorage.setItem('color-theme', 'dark');
        }
    }
});

// End Dark Mode
$.getJSON("../mobile/data/identitas.json", function(data){
    $("#login_deskripsi").append(data.login_deskripsi)
})  

// Login Member

function loginMember(){
    $("#login_member").show()
    $("#form_member").slideDown("slow", function(){
        $(this).show();
    });
}

function closeMember(){
    
    $("#form_member").slideUp("slow", function(){
        $(this).hide();
        $("#login_member").hide()
    });
}

function togglePassword(){

    var showPass = document.getElementById("showPassword")
    var hidePass = document.getElementById("hidePassword")
    var pass     = document.getElementById("password_user")
    showPass.classList.toggle("hidden")
    hidePass.classList.toggle("hidden")
    if(showPass.classList.contains("hidden"))
    {
        pass.setAttribute("type",'text')
    }else{
        pass.setAttribute("type",'password')

    }
}