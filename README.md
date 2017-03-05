# WIMCore
WIMCore is a library for the reading and writing of Windows Image files (WIM).

## DecryptESD
Remove encryption from the ESD files distributed through the Windows Store infrastructure with DecryptESD.

DecryptESD currently targets .NET Core 1.1.

A legacy version is available that uses .NET Framework 4.5, allowing it to be used back to Windows Vista SP2. Compatibility with the latest versions of Mono as of DecryptESD v0.3.2 has been confirmed on both Linux and OS X.

## Current State
* Currently being rewritten to use the current version of the .NET Core Framework.
* Unit tests are being added to the rewritten .NET Core version.
* .NET Core Rewrite also adds more generalised WIM library support in a new "WIMCore" Library.
* This new rewrite also adds a lot more sanity checks and error handling.

## Current State (Legacy version)
* ESDs are decrypted successfully, both pre-14361 and post-14361.
* ESDs will be almost identical byte-for-byte to esddecrypt output - the only differences will be due to DecryptESD dropping indentation in the XML data, which is what DISM does.
* XML and Integrity tables are corrected after decryption

## Future plans
* Localise into other languages

## Changelog (Legacy)
* v0.4.0
  * Allow update from a remote feed.
    * Defaults to new CryptoKey website XML feed, provided via BuildFeed team. Alternate feeds can be set via config or command argument.
  * Add command verbs `update` and `decrypt` to separate existing and new functionality.

* v0.3.2
  * No longer crash on already decrypted ESD - just skip it.
  * Add 14371 ESD key.
  * Swap from RijndaelManaged to AesCryptoServiceProvider for decryption.
    * This uses native APIs rather than the pure managed code implementation.
    * It is believed to use AES-NI instructions on supporting hardware on Windows.

* v0.3.1
  * Test all keys, not just "best" key - but favour the closest key in build number.
  * Add 14342 multi ESD key.
  * Reduce requirement to .NET Framework 4.5, down from 4.5.2.

* v0.3
  * Add basic command line switches.
  * Support for multiple ESDs in batch.
  * Some basic error handling.
  * Allow specifying a key via a command line switch.

* v0.2
  * Fix XML metadata.
  * Fix Integrity table.

* v0.1
  * Initial release.

## License
Copyright © 2017, Thomas Hounsell.

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
