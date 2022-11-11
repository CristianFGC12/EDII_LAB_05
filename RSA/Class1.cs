using System;
using System.Collections.Generic;
using System.Numerics;

namespace RSA
{
    public class RSA
    {
        public static List<int> GeneratePublicKey()
        {
            int p = 0;
            int q = 0;
            int numero;
            bool comprabacion = true;
            bool comprobacion2 = true;
            while (comprabacion)
            {
                Random random = new Random();
                numero = random.Next(3, 7);
                if (esPrimo(numero))
                {
                    p = numero;
                    comprabacion = false;
                }
                else
                {
                    comprabacion = true;
                }
            }
            while (comprobacion2)
            {
                Random random = new Random();
                numero = random.Next(3, 7);
                if (esPrimo(numero) && numero != p)
                {
                    q = numero;
                    comprobacion2 = false;
                }
                else
                {
                    comprobacion2 = true;
                }
            }
            int n = p * q;
            int z = (p - 1) * (q - 1);
            int k = 0;
            bool verificar = true;
            while (verificar)
            {
                Random random = new Random();
                numero = random.Next(2, z);
                if (esPrimo(numero))
                {
                    if (MCD(numero, z) == 1)
                    {
                        k = numero;
                        verificar = false;
                    }
                    else
                    {
                        verificar = true;
                    }
                }
                else
                {
                    verificar = true;
                }
            }
            List<int> list = new List<int>();
            list.Add(n);
            list.Add(k);
            list.Add(z);
            return list;
        }
        public static List<int> GeneratePrivateKey(List<int> numeros)
        {
            int j = 0;
            bool verificar = true;
            while (verificar)
            {
                Random random = new Random();
                j = random.Next(2, 25);
                int n = (numeros[1] * j) % numeros[2];
                if (n == 1 && j != numeros[1])
                {
                    verificar = false;
                }
                else
                {
                    verificar = true;
                }
            }
            List<int> privada = new List<int>();
            privada.Add(j);
            privada.Add(numeros[0]);
            return privada;
        }
        public static long Cipher(List<int> llave, long M)
        {
            long expo = llave[1];
            long k = binpow(M, expo);
            long c = k % llave[0];
            return c;
        }
        public static long Decipher(List<int> llave, long C) 
        {
            long expo = llave[1];
            long k = binpow(C, expo);
            long m = k % llave[1];
            return m;
        }
        public static int MCD(int num1, int num2) 
        {
            int a = Math.Max(num1, num2);
            int b = Math.Min(num1, num2);
            int res;
            do
            {
                res = b;
                b = a % b;
                a = res;
            } while (b != 0);
            return res;
        }
        public static bool esPrimo(int num) 
        {
            bool band = true;
            int m = 2;
            while ((band) && (m < num)) 
            {
                if(num % m == 0)
                    band = false;
                else
                    m=m+1;
            }
            if(band)
                return true;
            return false;
        }
        public static long  binpow(long a, long b)
        {
            long res = 1;
            long c = a;
            while (b > 0)
            {
                if (b == 1)
                    res = res * c;
                c = c * a;
                b--;
            }
            return res;
        }
    }
}
