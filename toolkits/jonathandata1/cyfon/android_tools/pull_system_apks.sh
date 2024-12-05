#! /bin/bash

################################################################
# Description: This program will pull all system apks from your android device and unpack them
#
# Author: Jonathan Scott Villarreal
# Date Modified: 07/17/2021
# Program Name: "CyFon Tools" (Android System APK Pull)
# Dependencies: This app requires, adb, and apktool, see README.md
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
    LOG_METHOD_2="${DIR}"/LOGS/CYFON/METHODS/2/LOGS/
    if [ ! -d "${LOG_METHOD_2}" ]; then
        mkdir -p "${LOG_METHOD_2}";
        cd  "${LOG_METHOD_2}" || return;
        mkdir "${input_dir}" && cd "${input_dir}" || exit

    elif [ -d "${LOG_METHOD_2}" ]; then
        cd "${LOG_METHOD_2}" || return
        mkdir "${input_dir}" && cd "${input_dir}" || exit

    else
        echo "Houston We HAVE A PROBLEM!!!"
    fi
# ---------------------------------------------------------------------------------------------
# Pulling all system apks
# ---------------------------------------------------------------------------------------------
    echo "This will allow you to pull all the system installed APKs. Output will be saved in the LOGS METHOD 2"
    double_space
        sleep 2
    system=$(adb shell 'pm list packages -s -f ' | awk -F 'package:' '{print $2}' | awk -F '=com' '{print $1}')

      for i in ${system}; do
          adb pull "${i}"
      done
# ---------------------------------------------------------------------------------------------
# unpacking all system apks
# ---------------------------------------------------------------------------------------------
      echo "This will decode all APKS in your current directory"
      echo "This can take some time"

      for i in *.apk; do apktool d "${i}"; done

      open .