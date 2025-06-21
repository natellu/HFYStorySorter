# HFYStorySorter

This is my hobby project. I like reading stories on r/hfy, but its hard to keep track what stories or chapters I already read.  
So I made this tool to help me organize and sort posts automatically.

## What it does

- Downloads new posts from r/hfy subreddit  
- Stores posts in a database (sqlite for now maybe I add postgre or something else)  
- Sorts posts into stories and chapters by analyzing post titles  


## How it works

The backend runs two background services:  
- PostFetcherService: downloads new posts from reddit API  
- StorySorterService: processes and sorts posts into stories and chapters using regex or AI (regex version barely works, the AI version is pretty decent but a lot slower. Use [natellu/storytitleparserllmapi](https://github.com/natellu/storytitleparserllmapi) to run the LLM API)
- Small webui, for now it only displays some stats like total chapters, stories, if the backend services are running, etc. Want to use the webui to be able to configure urls, timers, etc 


## Future plans

Create another project (probably react frontend) where I can
- See what posts I have read  
- Search and filter stories and chapters  
- Get notifications when new chapters are posted


