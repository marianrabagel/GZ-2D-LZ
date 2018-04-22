http://cs.fit.edu/~mmahoney/compression/

PAQ8 is a series of archivers that achieve very high compression rates at the expense of speed and memory.
 My latest version is paq8l, Mar. 8, 2007. I am no longer maintaining this code. 
However, there have been many compression improvements since then written by others,
 as described in the history section below.

  To compress:   paq8l -5 archive [files_or_folders...]  (creates archive.paq8p)
  To decompress: paq8l -d archive.paq8l [output_folder]

The -5 option is the default. It requires 233 MB of memory for compression and same for decompression.
 Options may range from -1 (35 MB) to -8 (1712 MB). More memory usually means better compression.
 In the Windows version you can also compress or decompress by putting paq8l.exe on the desktop 
and dropping files or folders on it. Compressed files/folders have a .paq8l extension.
 The input is not deleted.

NOTE: Files can only be decompressed with the same version of PAQ that they were compressed with.
 However, links to all versions can be found here.
 Decompression requires the same memory and time as compression. 