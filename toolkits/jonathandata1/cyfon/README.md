
# CyFon Tools
## Description: Hacking Tools - Bug Bounty, Mobile, Web, IOT, Cloud, Network, Hardware, ReCon Tools

### Author: Jonathan Scott Villarreal @jonathandata1
Distribution - Unlimited, you can use these tools anyway and anyhow you want to

## CURRENT VERSION 2.0
## NEW: Method 7 
> Subdomain Status Code, Get the status code for all subdomains and top level domain
## Update: Method 6 
> web based visual output



## VIDEO EXAMPLES

![CyFon 1.4 - Hacking Tools, Bug Bounty Tools, Android, Web Recon Tools](https://i.postimg.cc/C1Zt47mP/Untitled-design-Max-Quality-23.jpg)

[![CyFon 1.2 - Hacking Tools, Bug Bounty Tools, Android, Web Recon Tools](https://i.postimg.cc/Hsdmg1nH/cyfon-1-0.png)](https://youtu.be/iWTiGBtUciA)

[![CyFon Main - Hacking Tools, Bug Bounty Tools](https://i.postimg.cc/Vvf56NrY/cyfon-copy.jpg)](https://youtu.be/NNH6m625sb8A)

[![CWGET Master 1 - Hacking Tools, Bug Bounty Tools](https://i.postimg.cc/4x7R9R8D/Screen-Shot-2021-07-17-at-8-51-00-AM.png)](https://www.youtube.com/watch?v=YQPJH8pZvCM)

## Dependencies
 
####  If you are on on a MacOS you will need to install brew package manager
 ```
 /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```
#### Pup

 MacOS & Linux
 ```
 brew install pup
 ```
####  For a nice selection tool for the single apk download you will need to install peco 
 
 MacOS
 ```
 brew install peco
 ```
 Linux
 ```
 sudo apt install peco
 
 ```
#### To unpack the android apks you will need to install apk tool

 MacOS
 ```
 brew install apktool
 ```
 Linux
 ```
 sudo apt install apktool
 ```

####  ADB (Android Debug Bridge) must be installed is installed
 MacOS 
 ```
 brew install android-platform-tools
 ```
 
 Linux 
 ```
 sudo apt-get install android-tools-adb
 ```
 
####  CSV to Table
```
pip3 install csvtotable
```


> You also need to know how enable ADB for the device you are testing on.You also need to know how enable ADB for the device you are testing on.

## Tools Included So Far

**Sub Domain Scrape** - The program will pull down all subdomain data This is useful in bugbounty hunting because a lot of companies leave dev code on their servers, zip files, txt passwords and more

**Wget Master 1** - This line of code will pull all absolute URLS from whatever site you throw at it, it is a very clean 1 liner, you can add subdomains as well

**Pull All System APKs** - This program will pull all system apks from your android device and unpack them

**Pull Single APK** - This program will allow you to select and pull any apk on your device instead of downloading all. The apk will then be unpacked so you can start to search for vulnerabilities

**WADL PARSE** - This program will find the base of the .wadl path, and combine it with the resource paths to form full API endpoints for recon

**Dump All Android Secret Codes** - This will dump all android secret codes and show the system package associated to the secret code. 

Using the codes depends on your device

Here are the 3 ways 


