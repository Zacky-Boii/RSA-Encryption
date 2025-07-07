# RSA Encryption  

#### Video Demo:  
<https://www.youtube.com/watch?v=iGUJO4-P35s>  

#### Description: This program implements the RSA encryption algorithm in C#  

The idea for creating this program came from learning how data in transmitted and learning that it can be intercepted and read by the wrong person. This could include sensitive 
information so encryption is often used. Encryption is something that fascinated me as it used complicated maths to solve real-world problems.  

I first learned of asymmetric encryption is my school however we never went into detail on how it was implemented. I did some research and came across the RSA Encryption algorithm. 
This intrigued me as it used very large prime numbers to generate keys. I had previously created a program that generated all prime numbers up to the 32-bit signed integer limit using the sieve of Eratosthenes however, I soon learned that RSA uses prime numbers that are 1024 bits long: 32x bigger than the ones I had created.  

I researched how such large prime numbers were even generated as in seemed infeasible to me at the time. I stumbled across Fermat’s Primality Test but soon discovered that it was prone
to misidentification of numbers such as the Carmichael numbers. This led to be discovering the Miller-Rabin primality test and I found that it was similar to Fermat’s primality test 
however it was more accurate.  

I initially created the program assuming that the base, a, did not have to change so I kept it as 2. This led to misidentification of composite numbers as prime. After more research,
I found that the number a had to be randomly generated and between 1<a<n-1. I created the program using the integer variable type to test so I was able to use the Random function 
built into c#.  

When I attempted to make the program handle larger number, I was faced with the issue that it didn’t work with the BigInteger variable type. This led to me having to devise a way to 
generate large random numbers. I researched how the BigInteger variable type worked and found that it dynamically allocated memory as needed. It did this using the byte variable type
so I found a way to randomly fill an array of bytes. I then was able to check if it satisfied the condition 1<a<n – 1 and if it did not satisfy the condition, I would regenerate.  

To do this, I had to make sure that the number generated did not have more bits than n so I used the .GetBitLength() function which is integrated into the BigInteger data type. I then
added 7 to the bit length and did an integer division on that value and 8 to ensure that it was always the correct byte size.  

After the prime checker was working, I made it so it checked 5 times as the Miller-Rabin test is a probabilistic test so it is not always guaranteed. If the number is found to be
composite, however, it is guaranteed that it is composite. If the number was found to be composite within 5 tests, it would generate a new number and ensure that it was prime. 
With 5 tests with different values for a, the chance of accurately identifying a prime was 99.9%.  

After my Miller-Rabin test was working, I had to generate the values for the RSA Encryption algorithm. It was easy to generate the 2 primes needed as I just used the Miller-Rabin test
mixed in with a 1024 bit random number generator. This allowed me to generate 2 primes and then calculate n by multiplying them together.  

To generate the carmichaels totient, I used Eulers Euclidian formula to find the hcf’s and then used the fact that lcm(a,b) = a * (b/hcf(a,b)). This allowed me to then find a value for
the public exponent ‘e’ which had to be in the range 1<e<λ(n)-1 so I used the same method for generating a value for ‘a’ however I adapted it so that it only allowed numbers coprime
to λ(n).  

Now came the biggest challenge: calculating the private exponent ‘d’. Although it is easy to do by hand, I found it extremely difficult to write an algorithm for it. It involved 
substitution of previously calculated values. I initially thought of recursion but then realised that this could potentially lead to stack overflow especially with large numbers. 
I then decided on iteration, but it was very challenging.  

After multiple written examples, all deeply analysed, I noticed a pattern and found that the way could calculate it was identical to how the remainder was calculated. I think this 
part took me around 8 hours of playing around and analysing the formula and doing many examples by hand. This part was frustrating as 2 or 3 times I found a formula that worked and 
looked correct but only worked for that specific set of numbers.  

After finally calculating all numbers that was needed for RSA, I needed to now make my program functional. I decided on saving any keys and encryptions to a text file as this can 
simulate how to use RSA as I can create a set of keys for myself and then share my public key with the person sending me a message. They can encrypt the message and send me the 
encryption. I can then use my private key to view the message and even if the ciphertext got intercepted, it was computationally secure.  

Overall this project has taught me the importance of abstraction as it allowed me to stay focused and motivated. Moreover, it highlighted my joy for problem solving especially when 
I found an algorithm to calculate the private exponent.  
