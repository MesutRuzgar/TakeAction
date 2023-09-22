﻿using AksiyonAlma.Models;
using Business.Concrete;
using Entity.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AksiyonAlma.Controllers
{
    public class HomeController : Controller
    {
        StandartDuruslarManager standartDuruslarManager = new StandartDuruslarManager();
        UretimOperasyonBildirimleriManager uretimOperasyonBildirimleri = new UretimOperasyonBildirimleriManager();

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult AksiyonAl()
        {
            var uretim = uretimOperasyonBildirimleri.uretimListe();
            var mola = standartDuruslarManager.duruslarListe();

            List<UretimOperasyonBildirimleri> yeniOperasyonBildirimleri = new List<UretimOperasyonBildirimleri>();
            foreach (var u in uretim)
            {

                var duruslar = mola.Where(d => d.Baslangic.TimeOfDay >= u.Baslangic.TimeOfDay && d.Bitis.TimeOfDay <= u.Bitis.TimeOfDay).ToList();
                if (duruslar.Count > 0)
                {
                    DateTime oncekiBitis = u.Baslangic;

                    foreach (var d in duruslar)
                    {
                        if (oncekiBitis.TimeOfDay == d.Baslangic.TimeOfDay)
                        {
                            yeniOperasyonBildirimleri.Add(new UretimOperasyonBildirimleri
                            {
                                KayitNo = u.KayitNo,
                                Baslangic = d.Baslangic,
                                Bitis = d.Bitis,
                                ToplamSure = d.Bitis.TimeOfDay - d.Baslangic.TimeOfDay,
                                Statu = StatuEnum.Durus,
                                DurusNedeni = d.DurusNedeni
                            });
                        }
                        else
                        {
                        yeniOperasyonBildirimleri.Add(new UretimOperasyonBildirimleri
                        {
                            KayitNo = u.KayitNo,
                            Baslangic = oncekiBitis,
                            Bitis = d.Baslangic,
                            ToplamSure = d.Baslangic.TimeOfDay - oncekiBitis.TimeOfDay,
                            Statu = u.Statu,
                            DurusNedeni = u.DurusNedeni
                        });
                        yeniOperasyonBildirimleri.Add(new UretimOperasyonBildirimleri
                        {
                            KayitNo = u.KayitNo,
                            Baslangic = d.Baslangic,
                            Bitis = d.Bitis,
                            ToplamSure = d.Bitis.TimeOfDay - d.Baslangic.TimeOfDay,
                            Statu = StatuEnum.Durus,
                            DurusNedeni = d.DurusNedeni
                        });
                        }
                        oncekiBitis = d.Bitis;
                    }
                    if (oncekiBitis.TimeOfDay < u.Bitis.TimeOfDay)
                    {
                        yeniOperasyonBildirimleri.Add(new UretimOperasyonBildirimleri
                        {
                            KayitNo = u.KayitNo,
                            Baslangic = oncekiBitis,
                            Bitis = u.Bitis,
                            ToplamSure = u.Bitis.TimeOfDay - oncekiBitis.TimeOfDay,
                            Statu = u.Statu,
                            DurusNedeni = u.DurusNedeni
                        });
                    }
                }
                else
                {
                    yeniOperasyonBildirimleri.Add(u);
                }
            }
            var values = JsonConvert.SerializeObject(yeniOperasyonBildirimleri);
            return Json(values);
        }
    }


}



