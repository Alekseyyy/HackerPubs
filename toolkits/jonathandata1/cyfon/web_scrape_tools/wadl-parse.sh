#! /bin/bash
################################################################
# Description: The program will parse a .wadl file and show the api endpoints
# This is useful in bug bounty hunting because some endpoints should not be exposed
#
# Author: Jonathan Scott Villarreal
# Date Modified: 07/26/2021
# Program Name: "CyFon Tools" (Wadl Parser)
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

    # This will ask you to enter the URL you want to pull down
    # DO NOT put http
    # Do NOT put www
    # You will be able to put subdomain.domain.com or just domain.com
    echo "This will parse any .wadl file you throw at it, the extension must be .wadl"
    double_space
    IFS= read -r -p "Enter The URL to parse the .wadl file: " input
    double_space

    #I had to write an input_correction variable so that the dir being made would
    # not cause an error. You can't have / (forward slashes) as a directory name
    # this is a little messy but i'll clean it up later

    http_logic=$(echo "${input}" | grep -Ei "http" | wc -l )

    ic_out_dir(){

      if  [ "${http_logic}" -eq 1 ]; then
          echo "${input}" | sed -e 's/\//-/g' | awk -F '--' '{print $2}'
      elif  [ "${http_logic}" -eq 0 ]; then
          echo "${input}"
      else
          echo "Houston we have a problem!"
      fi
    }

    ic_out_dir=$(ic_out_dir)

    input_dir="${ic_out_dir}"_"${sessionid}"_"${time_date}"
    LOG_METHOD_4="${DIR}"/LOGS/CYFON/METHODS/4/LOGS
    if [ ! -d "${LOG_METHOD_4}" ]; then
        mkdir -p "${LOG_METHOD_4}";
        cd  "${LOG_METHOD_4}" || return;
        mkdir "${input_dir}" && cd "${input_dir}" || exit

    elif [ -d "${LOG_METHOD_4}" ]; then
        cd "${LOG_METHOD_4}" || return
        mkdir  "${input_dir}" && cd "${input_dir}" || exit

    else
        echo "Houston We HAVE A PROBLEM!!!"
    fi


# ---------------------------------------------------------------------------------------------
    ic_out_curl(){

      if  [ "${http_logic}" -eq 1 ]; then
          echo "${input}"
      elif  [ "${http_logic}" -eq 0 ]; then
          echo "https://${input}"
      else
          echo "Houston we have a problem!"
      fi
    }

     ic_out_curl=$(ic_out_curl)

        # The Base resource is always the most important part when parsing the .wadl file so we're pulling this out first and storing it in a temp dir
        curl "${ic_out_curl}" | grep -iE "resources base=" | grep -o '"[^"]\+"' | sed 's/"//g' | sed -e 's#/$##' > /tmp/base.txt
        # the resource path is where we see all the good stuff! So we'll pull those paths out and store them in a temp dir as well
        curl "${ic_out_curl}" | grep -iE "resource path=" | grep -o '"[^"]\+"' | sed 's/"//g'  > /tmp/path.txt

        base=$(cat /tmp/base.txt)
        path=$(cat /tmp/path.txt)

          wadl_cat(){



            for i in ${path}; do
                echo "${base}""${i}"
            done
    }

  wadl_cat | tee wadl.out
  open .