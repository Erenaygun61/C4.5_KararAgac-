using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KararAğacı
{
    class SayıSütunlar
    {
        public string isim = "";
        public int konum = 0;
        List<SayıSınıfı> SınıfList = new List<SayıSınıfı>();

        public SayıSütunlar(string isi,int kon)
        {
            isim = isi;
            konum = kon;
        }

        public string başlık()  //Ağacı Yazdırmak için başlıklarda kullanılacak fonksiyon
        {
            string BaşlıkS = "";
            for (int i = 0; i < SınıfList.Count; i++)
            {
                var item = SınıfList[i];
                BaşlıkS += item.isim + ":" + (item.ToplamDeğer / (double)item.sayı).ToString();
                if (i != SınıfList.Count-1)
                {
                    BaşlıkS += "-";
                }
            }
            return "(" + BaşlıkS + ")";
        }

        public void DiziyeEkle(string sınıf,double değer)
        {
            foreach (var item in SınıfList)
            {
                if (item.isim==sınıf)
                {
                    item.ToplamDeğer += değer;
                    item.sayı += 1;
                    return;
                }
            }
            SınıfList.Add(new SayıSınıfı(sınıf,1,değer));
        }
        public string enYakın(double num) {
            double fark = Math.Abs( (SınıfList[0].ToplamDeğer/(double)SınıfList[0].sayı)-num );
            string değer = SınıfList[0].isim;
            foreach (var item in SınıfList)
            {
                double arafark = Math.Abs(item.ToplamDeğer/(double)item.sayı -num);
                if (arafark < fark)
                {
                    fark = arafark;
                    değer = item.isim;
                }
            }
            return değer;
        }
    }
}
