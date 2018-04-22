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
