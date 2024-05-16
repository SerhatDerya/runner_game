<?php
ob_start();
?>

<!DOCTYPE html>
<html lang="en">

  <head>

    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">

    <link href="https://fonts.googleapis.com/css2?family=Open+Sans:wght@300;400;500;600;700;800&display=swap" rel="stylesheet">

    <title>how2play - giriş</title>

    <!-- Bootstrap core CSS -->
    <link href="vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">


    <!-- Additional CSS Files -->
    <link rel="stylesheet" href="assets/css/fontawesome.css">
    <link rel="stylesheet" href="assets/css/templatemo-tale-seo-agency.css">
    <link rel="stylesheet" href="assets/css/owl.css">
    <link rel="stylesheet" href="assets/css/animate.css">
    <link rel="stylesheet"href="https://unpkg.com/swiper@7/swiper-bundle.min.css"/>

    <link rel="stylesheet" type="text/css" href="form.css">
<!--

TemplateMo 582 Tale SEO Agency

https://templatemo.com/tm-582-tale-seo-agency

-->
  </head>

<body class="bg">

  <!-- ***** Preloader Start ***** -->
  <div id="js-preloader" class="js-preloader">
    <div class="preloader-inner">
      <span class="dot"></span>
      <div class="dots">
        <span></span>
        <span></span>
        <span></span>
      </div>
    </div>
  </div>
  <!-- ***** Preloader End ***** -->

  <!-- ***** Pre-Header Area Start ***** -->
  <div class="pre-header" id="top">
    <div class="container">
      <div class="row">
        <div class="col-lg-8 col-sm-9">
          <div class="left-info">
            <ul>
              <li><a href="#"><i class="fa fa-phone"></i>+90 123 567 89 12</a></li>
              <li><a href="#"><i class="fa fa-envelope"></i>how2playUNO@email.com</a></li>
              <li><a href="#"><i class="fa fa-map-marker"></i>Küçükçekmece/İstanbul</a></li>
            </ul>
          </div>
        </div>
        <div class="col-lg-4 col-sm-3">
          <div class="social-icons">
            <ul>
              <li><a href="https://tr-tr.facebook.com/"><i class="fab fa-facebook"></i></a></li>
              <li><a href="https://twitter.com/"><i class="fab fa-twitter"></i></a></li>
              <li><a href="https://tr.linkedin.com/"><i class="fab fa-linkedin"></i></a></li>
              <li><a href="https://www.google.com/intl/tr/account/about/"><i class="fab fa-google-plus"></i></a></li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  </div>
  <!-- ***** Pre-Header Area End ***** -->

  <!-- ***** Header Area Start ***** -->
  <header class="header-area header-sticky">
    <div class="container">
        <div class="row">
            <div class="col-12">
                <nav class="main-nav">
                    <!-- ***** Logo Start ***** -->
                    <a href="index.php" class="logo">
                        <img src="assets/images/logo.png" alt="" style="max-width: 212px;">
                    </a>
                    <!-- ***** Logo End ***** -->
                    <!-- ***** Menu Start ***** -->
                    <ul class="nav">
                      <li><a href="index.php">Ana Sayfa</a></li>
                      <li><a href="index.php">Nasıl Oynanır</a></li>
                      <li><a href="index.php">Keşfet</a></li>
                      <li class="has-sub">
                          <a href="javascript:void(0)">Sayfalar</a>
                          <ul class="sub-menu">
                              <li><a href="faqs.php">Kartlar</a></li>
                          </ul>
                      </li>
                      <li><a href="index.php">Ekstra</a></li>
                      <li><a href="index.php">İLETİŞİM</a></li>
                      <li><a href="about.php" >Kayıt ol/GİRİŞ yap</a></li>
                  </ul>   
                    <a class='menu-trigger'>
                        <span>Menu</span>
                    </a>
                    <!-- ***** Menu End ***** -->
                </nav>
            </div>
        </div>
    </div>
  </header>
  <!-- ***** Header Area End ***** -->

  <div class="arkaplan goruntu">
    <img src="assets/images/mainPhoto.png">
  </div>

 
 <form method="post" action="" class="goruntu">
<h5 style="color: ghostwhite;">Giriş<h5><br>
<input type="text" name="name" placeholder="isim" required="">
<input type="password" name="password" placeholder="şifre" required="">
<button type="submit" name="submit">GİRİŞ YAP</button>


<p class="message">Hesabınız yok mu?<a href="about2.php" style="color: #E9E954;"><u> Kayıt Ol </u></a></p><br><br><br><br><br>
</form>


  <footer>
    <div class="container">
      <div class="col-lg-12">
        <p>Copyright © 2036 <a href="#">how2play website</a>. All rights reserved. 
        
        <br>Design: <a href="https://templatemo.com" target="_blank">TemplateMo</a></p>
      </div>
    </div>
  </footer>


  <!-- Scripts -->
  <!-- Bootstrap core JavaScript -->
  <script src="vendor/jquery/jquery.min.js"></script>
  <script src="vendor/bootstrap/js/bootstrap.min.js"></script>

  <script src="assets/js/isotope.min.js"></script>
  <script src="assets/js/owl-carousel.js"></script>
  <script src="assets/js/tabs.js"></script>
  <script src="assets/js/popup.js"></script>
  <script src="assets/js/custom.js"></script>

  </body>

<?php
session_start();
include_once 'database.php';

if(isset($_POST["submit"]))
{

    $name= $_POST['name'];
    $password= $_POST['password'];

    $res=mysqli_query($link, "SELECT * FROM user1 WHERE name= '$name' && password='$password'");

    $res1=mysqli_num_rows($res);
    $_SESSION['username']=$name;

    if ($res1==1)
    {
        
        echo '<script> window.onload = function()
        {
            alert("Successfully Logged In");
        }</script>';
        
        header("location:index.php");
    
    }
    else
    {
    
        echo '<script> window.onload = function()
        {
            alert("Login ERROR, Wrong Name or Password");
        }</script>';
        
    }


}


?>


</html>

<?php
ob_end_flush();
?>