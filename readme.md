# How to use

- run ```docker compose -f kafka.yaml up -d```
- go to localhost:9021 (takes few secs to boot)
- go to Topics and create 2 default topics with names "actors" & "templates"
- run /publish endpoint in Actors service (takes ~8 mins)
- start "Processing" service first
- then start "Templates" service

## How to reset
- turn off either Templates or Processing service
- delete templates topic
- create templates topic
- run corresponding command
- run ```kafka-consumer-groups --bootstrap-server localhost:9092 --group templates --topic actors --reset-offsets --to-offset 0 --execute```
- run if exists ```kafka-consumer-groups --bootstrap-server localhost:9092 --group processing --topic templates --reset-offsets --to-offset 0 --execute```