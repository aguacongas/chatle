#!/usr/bin/env bash

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

ERRORS=()
SKIPPED=()

printf "${GREEN}Start tests\n"

for t in `grep -l "xunit.runner" test/*/project.json`; do
    TEST_DIR=$(dirname $t)

    _=$(grep "dnxcore50" $t)
    rc=$?
    if [[ $rc != 0 ]]; then
        printf "${YELLOW}Skipping tests on project ${TEST_DIR}. Project does not support CoreCLR${NC}\n"
        SKIPPED+=("${TEST_DIR} skipped")
        continue
    fi
    
    printf "${GREEN}Running tests on ${TEST_DIR}${NC}\n"

    (cd $TEST_DIR && dnvm run default -r coreclr test $@)
    rc=$?
    if [[ $rc != 0 ]]; then
        printf "${RED}Test ${TEST_DIR} failed error code ${rc}${NC}\n"
        ERRORS+=("${TEST_DIR} failed")
    fi
done

echo "============= TEST SUMMARY =============="

printf "${YELLOW}%s${NC}\n" "${SKIPPED[@]}"

if [ "${#ERRORS}" -ne "0" ]; then
    printf "${RED}%s${NC}\n" "${ERRORS[@]}"
    rc=1
else
    printf "${GREEN}All tests passed${NC}\n"
    rc=0
fi

exit $rc