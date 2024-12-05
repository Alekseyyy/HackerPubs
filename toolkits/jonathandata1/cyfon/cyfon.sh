#! /bin/bash

# ---------------------------------------------------------------------------------------------
#  Cyfon Tools
#  Creation Date: 07/17/2021
#  Description: Hacking Tools - Bug Bounty, Mobile, Web, IOT, Cloud, Network
#  Hardware,Author: Jonathan Scott Villarreal
#
# ---------------------------------------------------------------------------------------------

# ---------------------------------------------------------------------------------------------
# Dependencies List

    # printf "\n ${PURPLE}-------------------------- Checking For Dependencies -----------------------------------${NC}"
    # set -e
    # If you are on on a MacOS you will need to install brew package manager
    # brew install apktool

    # If you are on linux
    # sudo apt install apktool

    # You will also need to make sure adb is installed
    # linux and mac you can install adb with
    # pip3 install adb

    #    BREW_PACKAGES="apktool peco wget  curl pv"
    #    brew install ${BREW_PACKAGES} && true
    #    brew cask install ${BREW_PACKAGES_CASK} && true

# ---------------------------------------------------------------------------------------------
# ADDING SOME COLOR  & EASY FORMATTING TO THE TERMINAL OUTPUT
# ---------------------------------------------------------------------------------------------


    RED='\033[0;31m'
    PURPLE='\033[0;35m'
    YELLOW='\033[1;33m'
    ORANGE='\033[0;33m'
    CYAN='\033[1;36m'
    NC='\033[0m' # No Color

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
# PERSISTENT VARIABLES - DO NOT REMOVE!!!
# ---------------------------------------------------------------------------------------------

__dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

    #time_date=$(date +'%a-%h-%d-%Y-%I_%M_%S-%Z')

    # This will allow ctrl c to end the specific option number
    # without having to restart the full app
    # trap ctrl-c and call ctrl_c()


    trap ctrl_c INT

          function ctrl_c() {
            echo
            echo "Ctrl-C Ended the Application"
            # do the jobs
            exit
    }


    pause(){

        read -p "The selected process has completed. Press The 'Enter' Key, and select an option." fackEnterKey
    }





    double_space

    zero="Pull all files in all subdomains"
    zero() {

        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        printf "%s\n${CYAN} RUNNING METHOD #0 - ${zero} ${NC}"
        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        double_space
        trap ctrl_c INT

          function ctrl_c() {
            echo
            echo "Ctrl-C Ended the Application"
            # do the jobs
            exit
    }
        method_0(){


       sh "${__dir}"/web_scrape_tools/subdomain_scrape.sh



        }
            method_0

            pause
    }

    one="Wget Master 1 - Pull all urls from a webpage"
    one() {

        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        printf "%s\n${CYAN} RUNNING METHOD #1 - ${one} ${NC}"
        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        double_space
        trap ctrl_c INT

          function ctrl_c() {
            echo
            echo "Ctrl-C Ended the Application"
            # do the jobs
            exit
    }
        method_1(){

        sh "${__dir}"/wget_tools/wget_master_1.sh


        }
            method_1

        pause
    }

    two="Android - Pull All Android System APKs & Unpack Them"
    two (){

        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        printf "%s\n${CYAN} RUNNING METHOD #2 - ${two} ${NC}"
        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        double_space
        trap ctrl_c INT

          function ctrl_c() {
            echo
            echo "Ctrl-C Ended the Application"
            # do the jobs
            exit
    }
        method_2(){

         sh "${__dir}"/android_tools/pull_system_apks.sh


        }
            method_2

        pause
    }

    three="Android - Select & Download Any apk from your device and unpack it"
    three (){

        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        printf "%s\n${CYAN} RUNNING METHOD #3 - ${three} ${NC}"
        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        double_space
        trap ctrl_c INT

          function ctrl_c() {
            echo
            echo "Ctrl-C Ended the Application"
            # do the jobs
            exit
    }
        method_3(){

         sh "${__dir}"/android_tools/pull_single_apk.sh


        }
            method_3

        pause
    }

    four="Parse .wadl files and expose API Endpoints"
    four (){

        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        printf "%s\n${CYAN} RUNNING METHOD #4 - ${four} ${NC}"
        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        double_space
        trap ctrl_c INT

          function ctrl_c() {
            echo
            echo "Ctrl-C Ended the Application"
            # do the jobs
            exit
    }
        method_4(){

        sh "${__dir}"/web_scrape_tools/wadl-parse.sh


        }
            method_4

        pause
    }

    five="Dump All Android Secret Codes"
    five(){

        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        printf "%s\n${CYAN} RUNNING METHOD #5 - ${five} ${NC}"
        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        double_space
        trap ctrl_c INT

          function ctrl_c() {
            echo
            echo "Ctrl-C Ended the Application"
            # do the jobs
            exit
    }
        method_5(){

        sh "${__dir}"/android_tools/dump_android_secret_codes.sh


        }
            method_5

        pause
    }

    six="Subdomain Rake"
    six(){

        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        printf "%s\n${CYAN} RUNNING METHOD #6 - ${six} ${NC}"
        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        double_space
        trap ctrl_c INT

          function ctrl_c() {
            echo
            echo "Ctrl-C Ended the Application"
            # do the jobs
            exit
    }
        method_6(){

        sh "${__dir}"/web_scrape_tools/subdomain_rake.sh


        }
            method_6

        pause
    }

seven="Subdomain & Domain Status Code"
    seven(){

        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        printf "%s\n${CYAN} RUNNING METHOD #7 - ${seven} ${NC}"
        printf "%s\n${PURPLE}--------------------------------------------------------------${NC}"
        double_space
        trap ctrl_c INT

          function ctrl_c() {
            echo
            echo "Ctrl-C Ended the Application"
            # do the jobs
            exit
    }
        method_7(){

        sh "${__dir}"/web_scrape_tools/subdomain_status_codes.sh


        }
            method_7

        pause
    }

    ##############################################
    ##############################################
    show_menus() {
      clear

    printf "%s\n${PURPLE}##############################################################################################################################################################################${NC}"
    printf "%s\n${PURPLE}
                      _______           _______  _______  _
                     (  ____ \|\     /|(  ____ \(  ___  )( (    /|
                     | (    \/( \   / )| (    \/| (   ) ||  \  ( |
                     | |       \ (_) / | (__    | |   | ||   \ | |
                     | |        \   /  |  __)   | |   | || (\ \) |
                     | |         ) (   | (      | |   | || | \   |
                     | (____/\   | |   | )      | (___) || )  \  |
                     (_______/   \_/   |/       (_______)|/    )_)

    ${NC}"
    printf "%s\n${PURPLE}-----------------------------------------------------------------------------------------------------------------------------------------------------${NC}"
    printf "%s\n${PURPLE}-------------------------------------- Brought To You By: Jonathan Scott Villarreal ---------------------------------------------------------${NC}"
    printf "%s\n${PURPLE}-------------------------------------- Updated On: 8/26/2021 -------------------------------------------------------------------------------------${NC}"
    printf "%s\n${PURPLE}-------------------------------------- Version 1.5 -------------------------------------------------------------------------------------${NC}"
    printf "%s\n${PURPLE}-------------------------------------- Hacking Tools - Bug Bounty, Mobile, Web, IOT, Cloud, Network Hardware -------------------------${NC}"
    printf "%s\n${PURPLE}##############################################################################################################################################################################${NC}"


    double_space

	  printf "%s\n${PURPLE}-----------------------------------------------------------------------------------------------------------------------------------------------------${NC}"
    printf "%s\n${YELLOW} CyFon Menu ${NC}"
    printf "%s\n${PURPLE}-----------------------------------------------------------------------------------------------------------------------------------------------------${NC}"

    double_space

    echo "0. ${zero}"
    echo "1. ${one}"
    echo "2. ${two}"
    echo "3. ${three}"
    echo "4. ${four}"
    echo "5. ${five}"
    echo "6. ${six}"
    echo "7. ${seven}"
    echo "8. Exit"

    }
    # Read the user input

    read_options(){
      local choice
      read -p "Enter choice [ 0 - 8 ] " choice
      case ${choice} in
        0) zero ;;
        1) one ;;
        2) two ;;
        3) three ;;
        4) four ;;
        5) five ;;
        6) six ;;
        7) seven ;;
        8) exit 0;; #This will exit out of the application
		*) printf "%s\n${RED}Error...Option Not Valid, Please Choose Another${NC}" && sleep 2
	esac


    }

      # ----------------------------------------------
      # Close The Pipes and Stop Transmitting Data
      # ----------------------------------------------
      trap '' SIGINT SIGQUIT SIGTSTP

      # ----------------------------------------------
      # Initialize While Loops For The Main Logic
      # ----------------------------------------------
      while true
      do
        show_menus
        read_options
      done