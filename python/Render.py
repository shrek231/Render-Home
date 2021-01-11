import requests
import signal
import time
import json
import sys
import os

blenderpath = "blender"
filepath = "~/Desktop/blend/"
outputpath = "~/Desktop/op/"

frame = None
try:
    while True:
        frame =  requests.get("https://blenderrenderserver.youtubeadminist.repl.co/requestFrame").json()
        if frame != -1:
            print(f"Starting frame {frame}")
            os.system(f"curl -o {filepath}render.blend https://blenderrenderserver.youtubeadminist.repl.co/getBlend")
            os.system(f"{blenderpath} -b {filepath}render.blend  -o {outputpath} -f {frame}")
            os.system(f"curl -F {frame}=@C:\\tmp\\{frame:04}.png blenderrenderserver.youtubeadminist.repl.co/sendFrame")
            frame = None
            print(f"\nRendered frame {frame}")
        else:
            print("no frames to render, checking again in 10 seconds")
            time.sleep(10)

#except KeyboardInterrupt:
except:
    if frame != None and frame > 0:
        print("canceling frame "+ str(frame))
        os.system('curl https://BlenderRenderServer.youtubeadminist.repl.co/cancelFrame -d "{\\"frame\\":\\"' + str(frame) + '\\"}" -H "Content-Type: application/json"')
