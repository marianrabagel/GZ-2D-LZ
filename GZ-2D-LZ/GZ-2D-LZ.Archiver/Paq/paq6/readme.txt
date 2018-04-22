http://cs.fit.edu/~mmahoney/compression/paq.html

To create a new archive, you specify the name of the archive on the command line, 
and the files you want to compress, either after the archive name or from standard input.
Wildcards are not expanded in Windows, so you can use dir/b to get the same effect.
For example, to compress all.txt files into archive.pq6

paq6 archive.pq6 file1.txt file2.txt  (in any operating system)
paq6 archive.pq6 *.txt                (in UNIX)
dir/b*.txt | paq6 archive.pq6(in Windows)

To decompress:

paq6 archive.pq6

The -3 is optional, and gives a reasonable tradeoff. The possible values are:

Compression option  Memory needed to compress/decompress
------------------  ------------------------------------
 -0                   2 MB (fastest)
 -1                   3 MB
 -2                   6 MB
 -3                  18 MB (default)
 -4                  64 MB
 -5                 154 MB
 -6                 202 MB
 -7                 404 MB
 -8                 808 MB
 -9                1616 MB (best compression, slowest)

There are no decompression options. Instead, the compression option stored in the archive is used,
which means that the decompressor needs the same amount of memory as was used to compress the files. 
