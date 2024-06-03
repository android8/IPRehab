# Description: 
An .Net 8 web application for VA in patient rehabilitation evaulation with 3 stage performance indicators: initial, interim, and follow up.

# Architecture:
Client: MVC Razor page and TypeScript
Web API: MVC Restful with Swashbuckle test harnest

# Features:
1. automatically minimize all top level navigations and slide in on demand by hover over the page borders or sliding buttons
2. allow incrementally saving page data to the backend store
3. cache patient list for 24 hours to avoid expansive patient daily lookup
4. use TypeScript to interactively drive evaluation and automatically lock/unlock related evaluations by the business rules
5. use sectional navitation to allow jumping to the section by clicking section name in the left navation panel
6. realtime scoring of each question, sectional running total, and over all scores
7. slide in/out score card to show scoring matrix


