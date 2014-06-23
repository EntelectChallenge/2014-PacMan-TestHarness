2014-PacMan-TestHarness
=======================
Current version: v1.6
The Test harness for the 2014 Entelect R100K challenge.

The purpose of this project is allow for contestants to test their bots on their local machines. The test harness contains the logic for processing moves and running a match between two bots. This project can also be used to get a better understanding of the rules and help debug your bot.

This is version 1.6 of the test harness and improvements/enhancements to the code will be made over time, however the ruleset for the game will not change. The test harness has been made available to the community for peer review and bug fixes.

If you find any bugs or have any concerns, please email challenge@entelect.co.za with your modification requests and we'll have our technical panel review it and get back to you. 

-----------------------
Release Notes v1.6

*Changes to the mechanism for waiting for bot outputs.

Thanks for your contributions:

+bhaeussermann

-----------------------
Release Notes v1.5

*Fixed issue for instances where points are scored and a poison pill is dropped.

Thanks for your contributions:

+rm2k

-----------------------
Release Notes v1.4

*Fixed issue with consecutive respawns.

Thanks for your contributions:

+hwiechers

+rm2k

-----------------------
Release Notes v1.3

*Same-perspective console output

*Fix redirecting output on mono with osx

Thanks for your contributions:

+juanheyns

+bhaeussermann

-----------------------
Release Notes v1.2

*Moved symbols from project settings to constants for better character support.

Thanks for notifying us about the space issue:

+hwiechers

-----------------------
Release Notes v1.1

+Added better argument validation and help options.

+Added time buffer to reduce chance of reading state while a bot is writing it.

+Added enhanced output for further help with debugging.

+Added logic for edge case rule where players die consecutively.

*Fixed rule to disallow dropping poison pills in the respawn zone.

*General refactoring.

A special thanks to the guys who submitted enhancements. After inspection, we merged some of their changes with our own to create this release.

+Gustav

+hwiechers

-----------------------
Release Notes v1

+Initial test harness.
