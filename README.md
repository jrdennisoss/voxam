# Overview

This is an MPEG-1 analyzer tool I have been using to debug the ReelMagic game asset files.
It can be used to look inside and dissect MPEG-1 files containing a video elementary stream.

One could say that this tool "seperates out the different components" of a video file.



# Getting Started

Release downloads can be found on the GitHub release page:
XXX ADD URL HERE!!! XXX
XXX ADD URL HERE!!! XXX
XXX ADD URL HERE!!! XXX
XXX ADD URL HERE!!! XXX


Extract the release from the zip file and run the `Voxam.exe` file.



## To Convert a "Magical MPEG-1" File to a Standard MPEG-1 File

Voxam can be used to convert a "magical MPEG-1" file into a standard MPEG-1 file playable in
any standard MPEG-1 compliant media player. 

  * Open the file you want to convert (File -> Open)
  * Verify the file is a magical MPEG-1 file:
    * Open the "View" -> "Video Elementary Stream" view
    * Validate that the sequence bar (bottom blue bar) displays "Magical Sequence" and not "MPEG-1 Sequence"
  * Ensure the video plays without errors / artifacting:
    * Open the "View" -> "Video Elementary Stream" view
    * Click the "Play" button to verify the video plays as expected.
    * If not, update / adjust the settings under "Options" -> "ReelMagic Video Converter Settings"
  * Save / export a converted version of the file:
    * Open the "View" -> "Exporter" view
    * Click the "Convert and Save" button


## To View What Exactly the "ReelMagic Video Converter" is Correcting

The `f_code` values the video converter is updating can be viewed in list-formatted data under
the "View" -> "ReelMagic Transformation Viewer" view page. These values can be copied/pasted as
well as exported to a CSV list format. ("Options" -> "Export to CSV...")



# Building

Visual Studio Community 2022 was used to create this project.
There are two binaries that are produced:

  * The Voxam GUI Application (Voxam.exe)
  * A Specific-Purpose Variant of the PL\_MPEG Library. (VoxamPLMPEG.dll)


The `Voxam.sln` file is the master solution file which should be used for opening
this project in Visual Studio. 


To build the release:
  * Update the Voxam PL\_MPEG library version if applicable:
    (Solution Explorer -> Solution 'Voxam' -> VoxamPLMPEG -> Resource Files -> VoxamPLMPEG.rc
  * Update the Voxam GUI version:
    (Solution Explorer -> Solution 'Voxam' -> Voxam -> Properties -> AssemblyInfo.cs
  * Select "Release" / "Any CPU" for the build profile at the top
  * Choose "Build" -> "Rebuild Solution" from the menu.
  * Output files are in the "bin/x86/Release" folder.
  * AFAIK, `Voxam.exe` and `VoxamPLMPEG.dll` are the only two files needed.


