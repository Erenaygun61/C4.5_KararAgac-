using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace KararAğacı
{
    class Histogram
    {
        public string isim;
        public int sayı;
        public Histogram(string i,int s)
        {
            isim = i;
            sayı = s;
        }
        public static bool ListedeVarsaEkle(List<Histogram> Liste, string isim)
        {
            foreach (var item in Liste)
            {
                if (item.isim == isim)
                {
                    item.sayı += 1;
                    return true;
                }
            }
            Liste.Add(new Histogram(isim,1));
            return false;
        }
    }

    class GAIN
    {
        public string isim;
        public double değer;
        public List<Histogram> histo;
        public GAIN(string i,double d)
        {
            isim = i;
            değer = d;
        }
        public GAIN(string i, double d,List<Histogram> hlist)
        {
            isim = i;
            değer = d;
            histo = hlist;
        }
        public static GAIN EnBüyük(List<GAIN> gainList)
        {
            GAIN g=gainList[0];
            foreach (var item in gainList)
            {
                if (g.değer < item.değer)
                    g = item;
            }
            return g;
        }
    }

    class Program
    {

        public const char sep = ';';
        static void Main(string[] args)
        {
            
            string[] file = File.ReadAllLines("C:\\Users\\Eren\\Desktop\\KararAğacı\\egitim2.csv");
            string[] test = File.ReadAllLines("C:\\Users\\Eren\\Desktop\\KararAğacı\\test2.csv");
            Ağaç ağaç = new Ağaç();
            DizidenSayıÇıkar(file, "y");
            AğacıDoldur(ağaç, file, "y");

            ağacıBuda(ağaç);

            string dosyayazılacak=ağacıDosyayaYaz(ağaç);
            Console.WriteLine(tümünükarşılaştır(ağaç, test, "y"));

            File.WriteAllText("ağaçdosyası.txt", dosyayazılacak);

            Console.ReadLine();
        }
        public static string tümünükarşılaştır(Ağaç ağaç, string[] dizi, string cevapsütun)
        {
            int doğru = 0;
            int yanlış = 0;
            List<string> yanlışlar = new List<string>();
            for (int i = 1; i < dizi.Length; i++)
            {
                string item = dizi[i];
                if (karşılaştır(ağaç, item, dizi[0], cevapsütun))
                {
                    doğru += 1;
                }
                else
                {
                    yanlış += 1;
                    yanlışlar.Add(dizi[i]);
                }
            }
            File.WriteAllText("yanlışCevaplar.txt",string.Join("\r\n", yanlışlar.ToArray()));
            return "Doğru Sayısı= " + doğru.ToString() + " , Yanlış Sayısı= " + yanlış.ToString() + " , Başarı= % " + ((double)doğru / (doğru + yanlış)) * 100;
        }

        public static bool karşılaştır(Ağaç ağaç, string veri, string başlık, string cevapsütun)
        {
            string sütunisim;
            string[] değerler;
            bool sayımı = (ağaç.isim.Split('(')).Length > 1;
            if (sayımı)
                sütunisim = ağaç.isim.Split('(')[0];
            else
                sütunisim = ağaç.isim;
            int konum = Array.IndexOf(başlık.Split(sep), sütunisim);
            int cevapkonum = Array.IndexOf(başlık.Split(sep), cevapsütun);
            //List<Histogram> histoList = new List<Histogram>();
            if (sayımı)
            {
                değerler = ağaç.isim.Split('(')[1].Split(')')[0].Split('-');
                double fark = Math.Abs(double.Parse(değerler[0].Split(':')[1]) - double.Parse(veri.Split(sep)[konum]));
                string sonuc = değerler[0].Split(':')[0];
                foreach (var item in değerler)
                {
                    double arafark = Math.Abs(double.Parse(item.Split(':')[1]) - double.Parse(veri.Split(sep)[konum]));
                    if (arafark < fark)
                    {
                        fark = arafark;
                        sonuc = item.Split(':')[0];
                    }
                }
                if (ağaç.AltListe.Count > 0)
                {
                    foreach (var item in ağaç.AltListe)
                    {
                        if (item.değer == sonuc)
                        {
                            return karşılaştır(item, veri, başlık, cevapsütun);
                        }
                    }
                }
                else
                {
                    if (ağaç.sınıf == veri.Split(sep)[cevapkonum])
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (ağaç.AltListe.Count > 0)
                {
                    foreach (var item in ağaç.AltListe)
                    {
                        if (item.değer == veri.Split(sep)[konum])
                        {
                            return karşılaştır(item, veri, başlık, cevapsütun);
                        }
                    }
                }
                else
                {
                    if (ağaç.sınıf == veri.Split(sep)[cevapkonum])
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        public static string ağacıBuda(Ağaç ağaç)
        {
            if (ağaç.AltListe.Count == 0)
            {
                return ağaç.sınıf;
            }
            else
            {
                string sınıfdeğeri = "";
                bool budayımmı = true;
                foreach (var item in ağaç.AltListe)
                {
                    string değer = ağacıBuda(item);

                    if (değer == "") budayımmı = false;

                    if (sınıfdeğeri == "")
                    {
                        sınıfdeğeri = değer;
                    }
                    else
                    {
                        if (sınıfdeğeri != değer)
                        {
                            budayımmı = false;
                        }
                    }
                }
                if (budayımmı)
                {
                    ağaç.sınıf = sınıfdeğeri;
                    ağaç.AltListe = new List<Ağaç>();
                    return sınıfdeğeri;
                }

            }
            return "";
        }
        public static void DizidenSayıÇıkar(string[] dizi,string cevapsütun)
        {
            List<SayıSütunlar> SütunList = new List<SayıSütunlar>();
            string[] dizidenbul = dizi[1].Split(sep);
            int cevapkonum = Array.IndexOf(dizi[0].Split(sep),cevapsütun);
            for (int i = 0; i < dizidenbul.Length && i != cevapkonum; i++)
            {
                double değer = 0.0;
                if (double.TryParse(dizidenbul[i],out değer))
                {
                    SütunList.Add(new SayıSütunlar(dizi[0].Split(sep)[i],i));
                }
            }

            foreach (var item in SütunList)
            {
                for (int i = 1; i < dizi.Length; i++)
                {
                    double değer = double.Parse(dizi[i].Split(sep)[item.konum]);
                    item.DiziyeEkle(dizi[i].Split(sep)[cevapkonum], değer);
                }
            }

            foreach (var item in SütunList)
            {
                dizi[0] = yerinekoy(dizi[0], dizi[0].Split(sep)[item.konum] + item.başlık(),item.konum);
                for (int i = 1; i < dizi.Length; i++)
                {
                    double değer = double.Parse(dizi[i].Split(sep)[item.konum]);
                    string sınıfı = item.enYakın(değer);
                    dizi[i] = yerinekoy(dizi[i], sınıfı, item.konum);
                }
            }
        }

        public static void AğacıDoldur(Ağaç ağaç,string[] dizi,string cevapSütun)
        {
            string[] sonuçlar = new string[dizi.Length - 1];
            int konum = Array.IndexOf(dizi[0].Split(sep), cevapSütun);

            for (int i = 0; i < dizi.Length - 1; i++)
            {
                sonuçlar[i] = dizi[i + 1].Split(sep)[konum];
            }
            double EntropiG = Entropi(sonuçlar);

            if (EntropiG == 0)
            {
                ağaç.sınıf = dizi[1].Split(sep)[konum];
            }
            else
            {
                List<GAIN> GainList = Gain(dizi,cevapSütun);
                GAIN Enbüyük = GAIN.EnBüyük(GainList);
                ağaç.isim = Enbüyük.isim;
                if (Enbüyük.histo.Count == 1)
                {
                    ağaç.sınıf = dizi[1].Split(sep)[konum];
                    return;
                }
                foreach (var dal in Enbüyük.histo)
                {
                    Ağaç AltAğaç = new Ağaç();
                    AltAğaç.değer = dal.isim;

                    string[] AltDizi = altDizi(dizi, dal, Enbüyük.isim);

                    ağaç.AltListe.Add(AltAğaç);

                    AğacıDoldur(AltAğaç, AltDizi, cevapSütun);
                }

            }

        }
        
        public static void AğacıYaz(Ağaç ağaç,string boşluk="")
        {
            if (ağaç.AltListe.Count!=0)
            {
                foreach (var dal in ağaç.AltListe)
                {
                    Console.WriteLine(boşluk + ağaç.isim + ":" + dal.değer);
                    AğacıYaz(dal, boşluk + "    ");
                }
                
            }
            else
            {
                Console.WriteLine(boşluk + ağaç.sınıf);
            }
        }

        public static string ağacıDosyayaYaz(Ağaç ağaç,string boşluk="")
        {
            string ağaçstr = "";
            if (ağaç.AltListe.Count != 0)
            {
                foreach (var dal in ağaç.AltListe)
                {
                    ağaçstr += (boşluk + ağaç.isim + ":" + dal.değer + "\r\n");
                    //File.AppendAllText(dosya,boşluk + ağaç.isim + ":" + dal.değer+"\r\n");
                    ağaçstr += ağacıDosyayaYaz(dal, boşluk + "    ");
                }

            }
            else
            {
                ağaçstr += (boşluk + ağaç.sınıf + "\r\n");
                //File.AppendAllText(dosya,boşluk + ağaç.sınıf+"\r\n");
            }
            return ağaçstr;
        }

        public static List<GAIN> Gain(string[] dizi,string cevapSütun)
        {

            List<GAIN> GainList = new List<GAIN>();

            if (dizi == null || dizi.Length == 0 )
                return GainList;


            string[] sonuçlar = new string[dizi.Length - 1];
            int konum = Array.IndexOf(dizi[0].Split(sep), cevapSütun);

            for (int i = 0; i < dizi.Length - 1; i++)
            {
                sonuçlar[i] = dizi[i + 1].Split(sep)[konum];
            }
            double EntropiG = Entropi(sonuçlar);
            //Console.WriteLine("Genel Entropi " + EntropiG.ToString());

            foreach (var item in dizi[0].Split(sep))
            {
                if (item == cevapSütun) continue;
                konum=Array.IndexOf(dizi[0].Split(sep), item);

                string[] sütunlar = new string[dizi.Length - 1];

                for (int i = 0; i < dizi.Length - 1; i++)
                {
                    sütunlar[i] = dizi[i + 1].Split(sep)[konum];
                }

                List<Histogram> histo = new List<Histogram>();
                histo.Add(new Histogram(sütunlar[0], 0));

                for (int i = 0; i < sütunlar.Length; i++)
                {
                    Histogram.ListedeVarsaEkle(histo, sütunlar[i]);
                }

                double sütunEntropi = 0.0;
                foreach (var i in histo)
                {
                    string[] usütun = DizidenSec(dizi, i, item, cevapSütun);
                    double entropix = Entropi(usütun);
                    sütunEntropi += ((double)i.sayı/sütunlar.Length) * entropix;
                    //Console.WriteLine(i.sayı + " " + sütunlar.Length + " " + entropix);
                }
                GainList.Add(new GAIN(item,EntropiG - sütunEntropi,histo));
            }

            //Console.WriteLine(EntropiG.ToString());
            //Console.ReadLine();
            return GainList;
        }

        public static string[] DizidenSec(string[] dizi,Histogram histo,string sütuni,string sütuns)
        {
            int konum = Array.IndexOf(dizi[0].Split(sep), sütuni);
            int konums = Array.IndexOf(dizi[0].Split(sep), sütuns);
            string[] sütunlar = new string[histo.sayı];
            int count=0;

            for (int i = 0; i < dizi.Length; i++)
            {
                if (dizi[i].Split(sep)[konum] == histo.isim)
                {
                    sütunlar[count] = dizi[i].Split(sep)[konums];
                    count += 1;
                }
                
            }
            return sütunlar;
        }
        public static string yerinekoy(string eski,string yeni,int konum,int silinecekmi=0)
        {
            string giden = "";
            string[] eskidizi = eski.Split(sep);
            for (int i = 0; i < eskidizi.Length; i++)
            {
                if (i != konum)
                {
                    giden += eskidizi[i];
                    if (i !=eskidizi.Length-1)
                    {
                        giden += sep;
                    }
                }
                else
                {
                    giden += yeni;
                    if ((i != eskidizi.Length -1) && silinecekmi == 0)
                    {
                        giden += sep;
                    }
                }
            }
            return giden;
        }
        public static string[] altDizi(string[] dizi, Histogram histo,string sütunisim)
        {
            string[] sütunlar = new string[histo.sayı+1];

            int konum = Array.IndexOf(dizi[0].Split(sep), sütunisim);
            sütunlar[0] = yerinekoy(dizi[0], "", konum, 1);
            //sütunlar[0] = dizi[0].Replace(dizi[0].Split(sep)[konum] + sep, "").Replace(sep + dizi[0].Split(sep)[konum], "");
            int count = 1;
            for (int i = 0; i < dizi.Length; i++)
            {
                if (dizi[i].Split(sep)[konum] == histo.isim)
                {
                    sütunlar[count] = yerinekoy(dizi[i],"",konum,1);
                    //sütunlar[count] = dizi[i].Replace(dizi[i].Split(sep)[konum]+sep, "").Replace(sep+dizi[i].Split(sep)[konum], "");
                    count += 1;
                }

            }

            return sütunlar;
        }
        public static double Entropi(string[] dizi)
        {
            double EntropiDeğeri = 0.0;
            List<Histogram> histo = new List<Histogram>();
            histo.Add(new Histogram(dizi[0], 0));


            for (int i = 0; i < dizi.Length; i++)
            {
                Histogram.ListedeVarsaEkle(histo, dizi[i]);
            }
            

            foreach (var item in histo)
            {
                //Console.WriteLine(item.isim + " " + item.sayı);
                double olasılık = (double)item.sayı / (double)dizi.Length;
                EntropiDeğeri += -(double)(olasılık) * (Math.Log(olasılık) / Math.Log(2));
            }

            return EntropiDeğeri;
        }
        
    }
}
