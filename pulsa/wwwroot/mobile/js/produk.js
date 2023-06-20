$.getJSON("../mobile/data/produk.json", function(data){
    $.each(data, function(index, value){
            var produk  = `
                <div  id="value.id" class="item flex flex-col justify-center items-center p-2 bg-white rounded-lg shadow-md my-2"  onclick="$(${value.id}).toggle('hidden')">
                    <div class="">
                        <img src="${value.img}" alt="" class="h-14 bg-cover object-cover">
                    </div>
                    <h1 class="uppercase tracking-wider text-center text-xs md:text-sm text-gray-800 font-medium mb-2">
                        ${value.nama_produk}
                    </h1>
                    <button class="text-xs p-1 uppercase tracking-widest font-bold w-full rounded-lg " style="background: ${value.bg_color}; color:${value.font_color}">${value.harga}</button>
                </div>
            `
            $("#produk").append(produk)
           
            var fisik = value.voucher_fisik;
                if(fisik)
                {
                    fisik = ""
                }else{
                    fisik = "coret"
                }
            var bot = value.via_bot;
                if(bot)
                {
                    bot = ""
                }else{
                    bot = "coret"
                }
            var detailProduk = `
            <div class="fixed z-30 top-0 w-full left-0 hidden" id="${value.id}">
                <div class="flex items-center justify-center min-height-100vh text-center sm:block sm:p-0">
                    <div class="fixed inset-0 transition-opacity">
                        <div class="absolute inset-0 backdrop-blur" />
                    </div>
                    <div class="relative left-1/2 top-1/2 -translate-y-1/2 -translate-x-1/2 align-center bg-white rounded-lg text-left shadow-xl transform transition-all max-w-xs rounded-lg py-4">
                        <div class="bg-white rounded-lg mx-4">
                            <div class="bg-white justify-center items-center rounded-lg flex flex-col">
                                <h4 class="text-sm md:text-lg font-medium tracking-wider uppercase text-slate-600">${value.nama_produk}</h4>
                                <img src="${value.img}" alt="" class="h-32 bg-cover object-cover">
                            </div>
                            <div class="flex flex-col justify-end items-center mx-4">
                                <div class=" w-full">
                                ⏰ Masa Aktif <b>${value.masa_aktif}</b>
                                </div>
                                <div class="w-full">
                                ⚡ Speed up to <b>${value.speed}</b>
                                </div>
                                <div class="w-full">
                                📱 Dapat digunakan <b>${value.device_login} Devices</b>
                                </div>
                                <div class="w-full font-medium text-pink-400 ${fisik}">
                                🔖 Tersedia voucher fisik
                                </div>
                                <div class="w-full font-medium text-red-800 ${bot}">
                                🤖 bisa dibeli via Bot
                                </div>
                            </div>
                            <h1 class="text-3xl text-center font-bold my-2" > Rp. 5.000,- </h1>
                        </div>
                        <div class="flex flex-between justify-center items-center space-x-4 mx-2 mt-4">
                            <a href="${value.link_order}" class="w-1/2 text-center p-2 border shadow-md rounded-lg tracking-wider font-medium"> Beli </a>
                            <button onclick="$(${value.id}).toggle('hidden')" class="w-1/2 text-center p-2 bg-red-400 rounded-lg tracking-wider font-medium text-white"> Tutup </button>
                        </div>
                    </div>
                </div>
            </div>
            `
            $("#bottom").append(detailProduk)
        })
        $("#produk").owlCarousel({           
            loop:true,
            autoplay: true,
            autoplayTimeout: 3000,
            autoplayHoverPause: true,
            margin:10,
            nav:false,
            responsive : {
                0 : {
                items: 2,
                stagePadding: 50,
                },
                768 : {
                    items: 3,
                    stagePadding: 30,
                }
            },
            dots:false,
        })
    })
