const fs = require("fs");
const readline = require("readline");

fs.open("companylist.csv", "r", function(err, inputFd) {
    if (err) throw err;
    fs.open("processed-companylist.csv", "w", function(err, outputFd) {

        if (err) throw err;
        const writeStream = fs.createWriteStream("processed-companylist.csv");
        const rl = readline.createInterface(
            fs.createReadStream("companylist.csv"),
            null
        );

        rl.on("line", line => {
            writeStream.write(line.substring(0, line.length -1) + "\n");
        });
    });
});