#! /bin/bash
################################################################
# Description: The program will pull down all subdomain data
# This is useful in bugbounty hunting because a lot of companies
# leave dev code on their servers, zip files, txt passwords and more
#
# Author: Jonathan Scott Villarreal
# Date Modified: 07/14/2021
# Program Name: "Cyfon Tools" (Subdomain Data Pull)
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
    echo "This will pull all data, including files and subdirectories for any given domain. Output will be saved in the LOGS METHOD 0"
    double_space
    IFS= read -r -p "Enter The URL or File To Pull Down: " input
    double_space

    input_dir="${input}"_"${sessionid}"_"${time_date}"
    LOG_METHOD_0="${DIR}"/LOGS/CYFON/METHODS/0/LOGS/
    if [ ! -d "${LOG_METHOD_0}" ]; then
        mkdir -p "${LOG_METHOD_0}";
        cd  "${LOG_METHOD_0}" || return;
        mkdir "${input_dir}" && cd "${input_dir}" || exit

    elif [ -d "${LOG_METHOD_0}" ]; then
        cd "${LOG_METHOD_0}" || return
        mkdir "${input_dir}" && cd "${input_dir}" || exit

    else
        echo "Houston We HAVE A PROBLEM!!!"
    fi


# ---------------------------------------------------------------------------------------------



    ## I am using crt.sh since as of right now there is no search query limit and it is really accurate
    ## crt.sh checks the certificates associated to the domain and presents the subdomain data from the certs
    ## I am saving the output in a temp file on your localhost but you can keep this file if you want and remove /tmp

    curl "https://crt.sh/?q=${input}" > /tmp/out.html

    # for right now i'm only doing a grep for the following domain extensions
    # .gov|.mil|.com|.us|.net|.biz|.io|.org
    # I will make this more robust later
    # I am grepping the output and cleaning it up with sed
    # I am sorting the domains that were grepped and making sure there are no duplicates with the uniq command
    # I am again saving this cleaned up list to a temp file on your localhost

    cat /tmp/out.html | tr '<BR>' '\n' | grep -E ".gov|.mil|.com|.us|.net|.biz|.io|.org" | sed '/href/d;/crt.sh/d;/Type:/d;/[A-Z]=/d;/ /d' | sort | uniq > subdomains_out.txt
    # Finally I am using wget to pull all data, this is essentially scrapping the site
    # I am using --no-check-certificate because sometimes you will run into gov sites that will refuse to download
    # unless you are using the --no-check-certificate tag
    # using the refer and user-agent tags so that we're not blocked from download content, some sites check the User-Agent
    echo "Pull subdomain data for the following subdomains"
    cat subdomains_out.txt

    while read LINE; do  wget  --refer=http://google.com --user-agent="Mozilla/5.0 Firefox/4.0.1" --execute robots=off  --recursive  --no-parent --continue --no-clobber --timeout=1 --tries=1 --retry-connrefused  "${LINE}"  --no-check-certificate | xargs -n1 -P 100000; done < subdomains_out.txt
    open .
