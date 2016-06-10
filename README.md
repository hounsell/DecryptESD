# DecryptESD
Remove encryption from the ESD files distributed through the Windows Store infrastructure with DecryptESD.

It currently targets .NET Framework 4.6, though there's nothing too exotic about the code, so earlier frameworks should be fairly easy to add, and I'll likely also take it to .NET Core to take it cross-platform to Linux and OS X.

## Current State
* ESDs are decrypted successfully, both pre-14361 and post-14361.
* No fix up relating to the XML and integrity tables is performed, this is under development.
