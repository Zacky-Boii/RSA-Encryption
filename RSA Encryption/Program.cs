using System.Threading.Channels;
using System.Numerics;
using System.Security.Cryptography;
using System.Reflection.Metadata.Ecma335;
using System.Xml;
using System.CodeDom.Compiler;
using System.IO;

namespace RSA_Encryption
{
    internal class Program
    {

        public struct PrivateKey
        {
            public BigInteger n;
            public BigInteger d;
        }

        public struct PublicKey
        {
            public BigInteger n;
            public BigInteger e;
        }



        static void Main(string[] args)
        {

            int choice;

            PublicKey pubkey = new PublicKey();
            PrivateKey privkey = new PrivateKey();

            BigInteger[] encrypted = { };

            List<string> filenames = new List<string>();
            List<string> filelocations = new List<string>();

            int totalfiles = 0;

            do
            {
                Console.WriteLine("1. Generate new keys");
                Console.WriteLine("2. Export keys to text file");
                Console.WriteLine("3. Encrypt Message");
                Console.WriteLine("4. Export encrypted message to text file");
                Console.WriteLine("5. Import encrypted message from text file");
                Console.WriteLine("6. Decrypt message");

                if (!int.TryParse(Console.ReadLine()!, out choice))
                {
                    Console.WriteLine("Please enter a valid choice!");
                    continue;
                }

                if (choice == 1)
                {
                    Console.WriteLine("Generating new keys...");
                    (pubkey, privkey) = InitialiseKeys();
                    Console.WriteLine("Generated new keys!");
                }
                else if (choice == 2)
                {
                    SaveKeys(pubkey, privkey);
                }
                else if (choice == 3)
                {
                    encrypted = Encrypt();
                }
                else if (choice == 4)
                {
                    SaveCipher(encrypted);
                }
                else if (choice == 5)
                {
                    string name;
                    string location;
                    (name, location) = ImportCipher();
                    filenames.Add(name);
                    filelocations.Add(location);
                }
                else if (choice == 6)
                {
                    Decrypt(filenames, filelocations);
                }
                else Console.WriteLine("Enter a valid choice");
                Console.WriteLine();
            } while (choice != 7);

        }
        static BigInteger[] Encrypt()
        {
            Console.Write("Enter message to encrypt: ");
            string encrypt = Console.ReadLine()!;

            Console.WriteLine("Enter recipients public key's modulus (n): ");
            string n = Console.ReadLine()!;

            Console.WriteLine("Enter recipients public key's exponent (e): ");
            string e = Console.ReadLine()!;

            Console.WriteLine("Encrypting message...");
            BigInteger[] encrypted = new BigInteger[encrypt.Length];

            for (int i = 0; i < encrypt.Length; i++)
            {
                encrypted[i] = BigInteger.ModPow((int)encrypt[i], BigInteger.Parse(e), BigInteger.Parse(n));
            }

            return encrypted;
        }

        static void SaveCipher(BigInteger[] encrypted)
        {
            Console.WriteLine("What would you like to call the file?");
            string filename = Console.ReadLine()!;

            Console.WriteLine("Where would you like to save the file to?(e.g. C:\\School\\Projects\\RSA encryption)");
            string path = Console.ReadLine()!;

            string[] lines = new string[encrypted.Length];

            for (int i = 0; i < encrypted.Length; i++)
            {
                lines[i] = encrypted[i].ToString();
            }

            // Write the string array to a their file
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, filename + ".txt")))
            {
                foreach (string line in lines)
                {
                    outputFile.WriteLine(line);
                }
            }
        }

        static void SaveKeys(PublicKey pubkey, PrivateKey privkey)
        {
            Console.WriteLine("What would you like to call the file?");
            string filename = Console.ReadLine()!;

            Console.WriteLine("Where would you like to save the file to?(e.g. C:\\School\\Projects\\RSA encryption)");
            string path = Console.ReadLine()!;

            //if they put .txt at the end of their filename, this gets rid of it
            string[] parts = filename.Split('.');
            if (parts[parts.Length - 1] == "txt")
            {
                string temp = "";
                for (int i = 0; i < filename.Length - 4; i++)
                {
                    temp += filename[i];
                }
                filename = temp;
            }

            string pubkeytext = "PUBLIC KEY - n: " + pubkey.n + "\ne: " + pubkey.e;
            string privkeytext = "PRIVATE KEY - n: " + privkey.n + "\nd: " + privkey.d;

            //each member is a line
            string[] lines = { pubkeytext, privkeytext };

            // Write the string array to a their file
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, filename + ".txt")))
            {
                foreach (string line in lines)
                {
                    outputFile.WriteLine(line);
                }
            }
        }

        static (string, string) ImportCipher()
        {
            Console.WriteLine("What is the name of the text file?");
            string filename = Console.ReadLine()!;

            Console.WriteLine("What is location of the file? (e.g. C:\\School\\Projects\\RSA encryption)");
            string filelocation = Console.ReadLine()!;

            //if they put .txt at the end of their filename, this gets rid of it
            string[] parts = filename.Split('.');
            if (parts[parts.Length - 1] == "txt")
            {
                string temp = "";
                for (int i = 0; i < filename.Length - 4; i++)
                {
                    temp += filename[i];
                }
                filename = temp;
            }

            return (filename, filelocation);
        }

        static void Decrypt(List<string> filenames, List<string> filelocations)
        {
            Console.WriteLine("Imported files:");

            int number = 1;
            foreach (string file in filenames)
            {
                Console.WriteLine(number + ". " + file + ".txt");
                number++;
            }

            Console.Write("\nEnter number of file you would like to decrypt: \n");
            int filenumber = (int.Parse(Console.ReadLine()!) - 1);

            string path = filelocations[filenumber] + "\\" + filenames[filenumber] + ".txt";

            List<BigInteger> encrypted = new List<BigInteger>();

            StreamReader sr = new StreamReader(path);

            string line = sr.ReadLine();
            while (line != null)
            {
                encrypted.Add(BigInteger.Parse(line));
                line = sr.ReadLine();
            }

            sr.Close();

            Console.WriteLine("Enter your private key's modulus (n): ");
            string n = Console.ReadLine()!;

            Console.WriteLine("Enter your private key's exponent (d): ");
            string d = Console.ReadLine()!;

            Console.WriteLine("Decrypting message...");

            string decrypted = "";

            for (int i = 0; i < encrypted.Count; i++)
            {
                decrypted += (char)BigInteger.ModPow(encrypted[i], BigInteger.Parse(d), BigInteger.Parse(n));
            }
            Console.WriteLine(decrypted);
        }

        static (PublicKey, PrivateKey) InitialiseKeys()
        {
            BigInteger p = Generate1024bitPrime();
            BigInteger q = Generate1024bitPrime();
            BigInteger n = p * q;
            BigInteger carmichael = LCM(p - 1, q - 1);
            BigInteger e = GenerateE(carmichael);
            BigInteger d = CalculateD(e, carmichael);

            PublicKey publickey;
            PrivateKey privatekey;

            publickey.n = n;
            publickey.e = e;

            privatekey.n = n;
            privatekey.d = d;

            return (publickey, privatekey);
        }

        static BigInteger CalculateD(BigInteger e, BigInteger phi)//use modular multiplicative inverse
        {
            //MOD INVERSE!
            //we want e*oldCoefficient mod phi = 1
            BigInteger r = phi;
            BigInteger oldR = e;
            BigInteger coefficient = 0;
            BigInteger oldCoefficient = 1;

            while (r != 0)
            {
                BigInteger quotient = oldR / r; //calculate quotient
                BigInteger tempR = r; // store r in temporary value so we can transfer it to oldR when done
                r = oldR - quotient * r; // calculate new remainder
                oldR = tempR; // store the previous remainder in oldR

                BigInteger tempCoefficient = coefficient; // store c in temporary value so we can transfer it to oldC when done
                coefficient = oldCoefficient - quotient * coefficient; // calculate coefficient using same algorithm as remainder
                oldCoefficient = tempCoefficient; // store the previous coeffiecient in oldCoeffcient
            }
            return (oldCoefficient + phi) % phi;
            //IT WORKS!!!!!!
        }

        static BigInteger LCM(BigInteger p, BigInteger q)
        {
            BigInteger hcf = HCF(p, q);
            return p * (q / hcf);
        }

        static BigInteger HCF(BigInteger p, BigInteger q)
        {
            while (q != 0)
            {
                BigInteger temp = q;
                q = p % q;
                p = temp;
            }
            return p;
        }


        static BigInteger Generate1024bitPrime()
        {
            BigInteger prime = 0;

            while (!MillerRabinTest(prime.ToString()))
            {
                long bitlength = 1024;
                long bytelength = (bitlength + 7) / 8;
                byte[] bytes = new byte[bytelength];

                RandomNumberGenerator.Fill(bytes);
                prime = new BigInteger(bytes);
                BigInteger.Abs(prime);
            }
            return prime;
        }

        //INPUT MUST BE STRING
        static bool MillerRabinTest(string number)
        {
            Random stream = new Random();

            BigInteger n;
            if (!BigInteger.TryParse(number, out n)) return false;

            if (n < 2 || n % 2 == 0) return false;
            if (n == 2) return true;

            //need to calculate and initialise k and m

            //calulating m
            BigInteger m = n - 1;
            int k = 0;
            while (m % 2 == 0)
            {
                m /= 2;
                k++;
            }
            //Console.WriteLine("a="+a+" m="+m+" k="+k);

            //do check 5 times for 99.9% certainty
            int checks = 0;
            bool prime = true;
            while (prime == true && checks < 5)
            {
                prime = PrimeCheck(n, m, k);
                checks++;
            }
            if (prime) return true;
            return false;
        }

        static BigInteger GenerateE(BigInteger c)//upper bound exclusive
        {
            BigInteger max = c - 1;
            int min = 1;
            BigInteger e;

            do
            {
                long bitlength = c.GetBitLength();
                long bytelength = (bitlength + 7) / 8;
                byte[] bytes = new byte[bytelength];

                RandomNumberGenerator.Fill(bytes);
                e = new BigInteger(bytes);
                BigInteger.Abs(e);
            } while (e < min || e > max || !Coprime(e, c));

            return e;
        }

        static bool Coprime(BigInteger a, BigInteger b)//check if a and b are coprime
        {
            if (HCF(a, b) == 1) return true;
            return false;
        }

        static BigInteger GenerateValueForA(BigInteger n)
        {
            BigInteger max = n - 2;
            int min = 2;
            BigInteger a;

            do
            {
                long bitlength = n.GetBitLength();
                long bytelength = (bitlength + 7) / 8;
                byte[] bytes = new byte[bytelength];

                RandomNumberGenerator.Fill(bytes);
                a = new BigInteger(bytes);
                BigInteger.Abs(a);
            } while (a < min || a > max);

            return a;
        }

        static bool PrimeCheck(BigInteger n, BigInteger m, int k)
        {
            //generating a:
            BigInteger a = GenerateValueForA(n);

            //b value calculation
            BigInteger b = BigInteger.ModPow(a, m, n);
            if (b == 1) return true;

            int loops = 0;
            while (b != 1 && b != (n - 1) && loops != k)
            {
                b = BigInteger.ModPow(b, 2, n);
                loops++;
            }
            if (b == n - 1) return true;
            return false;
        }//actually checks if they are prime by calculating b values
    }
}

//TODO: currently, it works i think and the way i can will improve it is by calculating the b value 5 times
// if it detects it as composite even once, it is composite. I need to generate different a values but m and k
// will stay the same so create a seperate funtion for the b value calulation and run it 5 times - 05/07/25
//DONE: 06/07/25
