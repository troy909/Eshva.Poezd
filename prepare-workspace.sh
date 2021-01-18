#!/usr/bin/env bash

# Скрипт подготовки рабочего пространства (локального репозитория) к работе над проектом, согласно требованиям.

script_folder="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"


function setLineEnding {
    # Все файлы, которые будут попадать в object store репозитория, будут конвертироваться из CRLF в LF, но, если
    # файл в object store хранится с CRLF, при попадении в рабочую папку, CRLF не будет конвертирован в LF.
    git config core.autocrlf input
}

function addPreCommitMessageHook {
    # Создание hook для первичного формирования комментария к коммиту. Будет добавляться ссылка на issue ID задачи в Jira.
    # ВНИМАНИЕ: Для того, чтобы это работало правильно, необходимо, чтобы имя ветки не содержало ничего кроме issue ID.
    # Например, POEZD-34 допустимо, а POEZD-34-precommit-hook недопустимо.
    # ВНИМАНИЕ: Данный скрипт написан для проекта (в Jira) POEZD. Если необходимо задействовать его для другого
    # проекта, поменяйте переменную jiraProjectName.

    hook_file_name="$script_folder/.git/hooks/prepare-commit-msg"

cat << 'EOF' > $hook_file_name
#!/usr/bin/env bash

detachedMatch=$( git branch | grep -Ec 'no branch' )
if [ $detachedMatch -eq 0 ]
then
    message=$( cat $1 )

    # Append IssueID to commit if the branch name ends in /2-ABCD
    issueID=$( git symbolic-ref HEAD | awk -F '/' '{ print $NF }' | grep -E '\w+\-[0-9]+' )
    if [ $issueID ]
    then
        #Issue ID should be appended *only* if it's not mentioned in the commit message
        messageMatch=$( echo "$message" | grep -c "^$issueID" )
        if ! [ $messageMatch -gt 0 ]
        then
            jiraProjectName="POEZD"
            messageMatch2=$( echo "$message" | grep -Ec '^$jiraProjectName\-[0-9]+' )
            if ! [ $messageMatch2 -gt 0 ]
            then
                echo "$issueID $message" > $1
            fi
        fi
    fi

    # Remove IssueID from the merge message
    mergeMatch=$( echo "$2" | grep -Ec 'merge' )
    if [ $mergeMatch -gt 0 ]
    then
        # Both 'Merge' and full branch name are present in the commit message.
        # To avoid linking the merge commit to a YT issue/US review, we have to remove IssueID from the message.
        message=$( echo "$message" | sed -r 's,/\w+\-[0-9]+,,g' )
        echo "$message" > $1
    fi
fi
EOF

    # Hook-файл должен быть запускаемым, иначе Git проигнорирует его.
    chmod 755 $hook_file_name
}

function createLocalDataFolders {
    mkdir --parents .local-data/
}


setLineEnding
addPreCommitMessageHook
createLocalDataFolders
