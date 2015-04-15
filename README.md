Poker Hand History Parser
=========================

C# .NET library for parsing poker hand-histories.

What can it be user for?
------------------------

Parsing hand-histories is a common task in most 3rd party poker projects, however it has many edge cases, formats and is very tedious. The hand-parser was written to take poker hand-history text logs and parse summary information and/or hand-action information without having to deal with all the headaches. The parser was designed to be fairly fast, and in some places this may have affected code read-ability, however there are extensive unit tests.

Site Support
------------

The poker hand-history parser currently supports the following sites:

 * Poker Stars (.com / .fr / .it / .es)
 * Full Tilt Poker
 * Party Poker
 * On Game (.com / .fr / .it)
 * IPoker
 * Merge
 * 888
 * Microgaming
 * Winimax
 * Winning Poker
 * Boss Media
 * Entraction (deprecated)

Format Support
--------------
 * Cash games (2 to 10 players)
 * Zoom (Poker Stars Only)

Game Type Support
-----------------
 * No Limit Hold'em
 * Cap No Limit Hold'em
 * Fixed Limit Hold'em
 * Pot Limit Hold'em
 * Pot Limit Omaha
 * Fixed Limit Omaha
 * Omaha Eight of Better

Future Upgrades
---------------
 * Single and multi-table tournament support
 * Ongoing support

Documentation
-------------

See the [documentation](https://github.com/KBelbina/PokerHandHistoryParser/wiki).

License
-------

The MIT License

Copyright (c) 2014 "Luke" Kader Belbina

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
