using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KararAğacı
{
    class Ağaç
    {
        string Nitelik;
        List<Ağaç> Dallar;
        string değeri;
        public void düğümekle(Ağaç düğüm)
        {
            Dallar.Add(düğüm);
        }
        public Ağaç hangisi(string soru)
        {
            foreach (var item in Dallar)
            {
                if (item.değeri == soru)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
