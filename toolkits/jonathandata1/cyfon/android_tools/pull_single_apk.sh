#! /bin/bash

################################################################
# Description: This program will allow you to select and pull any apk on your device instead of downloading all
# The apk will then be unpacked so you can start to search for vulnerabilities
#
# Author: Jonathan Scott Villarreal
# Date Modified: 07/17/2021
# Program Name: "CyFon Tools" (Android Pull Any selected APK & unpack)
# Dependencies: This app requires, adb, and apktool, and peco  see README.md
################################################################

# This sessions id will be generated everytime you run the program and it will be saved as a folder name
# This is for organizational purposes

DIR="$(dirname "$(which "$0")")"


trap ctrl_c INT

 ctrl_c() {
    echo
    echo "Ctrl-C by user"
    # do the jobs
    exit
}

sessionid=$(uuidgen)
time_date=$(date +'%a-%h-%d-%Y-%I_%M_%S-%Z')

    # FUNCTION TO ADD 1 BLANK ROW JUST BY TYPING space
        space(){

                     echo ""

         }
     # FUNCTION TO ADD 2 BLANK ROWS JUST BY TYPING double_space
         double_space(){

                    echo ""
                    echo ""

         }

# ---------------------------------------------------------------------------------------------
# LOG FILE DIRECTORIES
# Every method has a logout put directory so you can go back and review
# ---------------------------------------------------------------------------------------------
device_mpn=$(adb shell getprop ril.product_code)

    input_dir="${device_mpn}"_"${sessionid}"_"${time_date}"
    LOG_METHOD_3="${DIR}"/LOGS/CYFON/METHODS/3/LOGS/
    if [ ! -d "${LOG_METHOD_3}" ]; then
        mkdir -p "${LOG_METHOD_3}";
        cd  "${LOG_METHOD_3}" || return;
        mkdir "${input_dir}" && cd "${input_dir}" || exit

    elif [ -d "${LOG_METHOD_3}" ]; then
        cd "${LOG_METHOD_3}" || return
        mkdir "${input_dir}" && cd "${input_dir}" || exit

    else
        echo "Houston We HAVE A PROBLEM!!!"
    fi
# ---------------------------------------------------------------------------------------------
# Pulling Any Selected System or User Installed Apk
# ---------------------------------------------------------------------------------------------
  # This is super messy I'll clean it up later

    echo "This will allow you to pull any selected APK. Output will be saved in the LOGS METHOD 3"
    double_space
      adb shell 'pm list packages -f' | awk -F 'package:' '{print $2}' | awk -F '=com' '{print $1}' | peco | xargs adb pull | tee /tmp/single_apk.txt

      rename=$(cat /tmp/single_apk.txt | awk -F 'com.' '{print $2}' | awk -F '-' '{print $1}')



# ---------------------------------------------------------------------------------------------
# unpacking all apks
# ---------------------------------------------------------------------------------------------
    double_space
      echo "This will decode all APKS in your current directory"
      echo "This can take some time"

      for i in *.apk; do apktool d "${i}"; done
      if [ -f base.apk ]; then
         mv base.apk "${rename}".apk
         mv base "${rename}"
      else
        exit 0
      fi

      open .