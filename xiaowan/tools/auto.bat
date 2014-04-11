"D:\marukotoolbox\marukotoolbox\xiaowan\tools\x264_32_tMod-8bit-420.exe"  --crf 24 --preset 8 --demuxer lavf -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8  -o "E:\Video\Happy Xmas_Batch.mp4" "E:\Video\Happy Xmas.mp4" --acodec faac --abitrate 128 

@echo off&&echo MsgBox "Finish!",64,"Maruko Toolbox" >> msg.vbs &&call msg.vbs &&del msg.vbs
cmd