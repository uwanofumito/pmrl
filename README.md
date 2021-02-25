This program is based on the following paper. <br>
https://www.jstage.jst.go.jp/article/jcmsi/11/4/11_321/_article/-char/en

This program requires two files, config.csv and envData.csv in 'Release/netcoreapp3.1/' directory.
config.csv includes configs of the proposed method and other. envData.csv includes the details of the environments. This table have some parameters, initial learning iterations, width of env., height of env., the number of agents, the num. of goals, the locations of starts, the locations of goals in order from the left.
the locations of starts have the form which 'width'x'height'x'the num. of agents', and those of goals have the form which  'width'x'height'x'reward'.
This program performs with the above two files in the same hierarchy.

This PMRL has two modes, on and off communication modes. The 'Communication' column in config.csv changes PMRL modes.
The agents share the stored steps in the on communication mode, on the other hand, the agents calculate the steps of others based on frequency of reaching goals in the off communication mode. The agents calculates the equations (2) and (3) based on those steps.
