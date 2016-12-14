using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KararAğacı
{
    class SayıSınıfı
    {   //Cevap Sınıflarına göre Sınıfların ilgili sütundaki değerlerinin toplanmı ve ortalamasını elde etmek için
        public string isim = "";
        public int sayı = 0;
        public double ToplamDeğer=0.0;
        public SayıSınıfı(string isi,int say,double top)
        {
            isim = isi;
            sayı = say;
            ToplamDeğer = top;
        }
    }
}
