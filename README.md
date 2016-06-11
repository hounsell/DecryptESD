# DecryptESD
Remove encryption from the ESD files distributed through the Windows Store infrastructure with DecryptESD.

DecryptESD currently targets .NET Framework 4.5.2, allowing it to be used back to Windows Vista SP2. I've not tested compatibility with Mono, but I don't evisage there being an issue, given that it's a self-contained assembly with no dependencies beyond the .NET Framework itself. I also plan to take it to .NET Core and test it on OS X and Linux.

## Current State
* ESDs are decrypted successfully, both pre-14361 and post-14361.
* ESDs will be almost identical byte-for-byte to esddecrypt output - the only differences will be due to DecryptESD dropping indentation in the XML data, which is what DISM does.
* XML and Integrity tables are corrected after decryption

## Future plans
* Unit testing is probably a good idea and entirely implementable
* .NET Core support - include testing on OS X and Linux
* Extend functionality
* Localise into other languages
* oh, and error handling is probably pretty important too.

## License
Copyright © 2016, Thomas Hounsell.

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.