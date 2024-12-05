#! /bin/bash

################################################################
# Description: This program will dump all android secret codes from the device
# adb must be enabled, and sometimes you may not get a response if there is a custom build
#
# Author: Jonathan Scott Villarreal
# Date Modified: 08/06/2021
# Program Name: "CyFon Tools" (Dump Android Secret Codes)
# Dependencies: This app requires, adb see README.md
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
man=$(adb shell getprop | grep ro.product.manufacturer | tr -d '[]' | awk '{print $NF}')
model=$(adb shell getprop ro.product.model)
os_ver=$(adb shell getprop ro.build.version.release)

    input_dir="${sessionid}"_"${time_date}"
    LOG_METHOD_5="${DIR}"/LOGS/CYFON/METHODS/5/LOGS/
    if [ ! -d "${LOG_METHOD_5}" ]; then
        mkdir -p "${LOG_METHOD_5}";
        cd  "${LOG_METHOD_5}" || return;
        mkdir "${input_dir}" && cd "${input_dir}" || exit

    elif [ -d "${LOG_METHOD_5}" ]; then
        cd "${LOG_METHOD_5}" || return
        mkdir "${input_dir}" && cd "${input_dir}" || exit

    else
        echo "Houston We HAVE A PROBLEM!!!"
    fi
# ---------------------------------------------------------------------------------------------
# Dumping all android secret codes
# ---------------------------------------------------------------------------------------------

    echo "This will dump all the android secret codes from the device, sometimes you may not get any if there is a custom build"
    android_secret_code_dump(){

      package_name_trim=$(adb shell 'pm list packages -s -f' | awk -F 'package:' '{print $2}' | awk -F '=' '{print $2}')

      # list the system package name
      # list the android_secret_code right below it
                for i in ${package_name_trim}; do
                  echo "${i}" && adb shell pm dump "${i}" | grep -E 'Scheme: "android_secret_code"|Authority: "[0-9].*"|Authority: "[A-Z].*"'
                done
      }

      android_secret_code_dump | tee "${man}"_"${model}"_"${os_ver}"_android_secret_codes.log
      open .
