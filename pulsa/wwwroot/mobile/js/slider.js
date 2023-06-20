$.getJSON("../mobile/data/slider.json", function(data){
    $.each(data.banner, function(index, value){
            var slider  = `
                <div class="item  text-gray-50">
                    <div class="relative bg-blue-400 dark:bg-sky-800 rounded-lg shadow-md h-40">
                        <img src="${value.gambar}" class="rounded-lg bg-cover object-cover h-40" >
                    </div>
                </div>
            `
            $("#slider").append(slider)
        })
        $("#slider").owlCarousel({
            stagePadding: 40,
            loop:true,
            autoplay: true,
            autoplayTimeout: 3000,
            autoplayHoverPause: true,
            margin:10,
            nav:false,
            items:1,
            dots:false,
        })
        
    })
    $("#paket").owlCarousel({
        stagePadding: 30,
        loop:true,
        autoplay: true,
        autoplayTimeout: 3000,
        autoplayHoverPause: true,
        margin:10,
        nav:false,
        items:3,
        dots:false,
    })