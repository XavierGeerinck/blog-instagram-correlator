const fs = require('fs');
const delay = require('delay');
const config = require('./config');
const API = require('./API/CognitiveServiceVision');
const dataFolder = './data/';
const outputFolder = './output/';

const api = new API(config.thirdParty.cognitiveServices.imageAnalyze.apiKey);

// Get list of images in path with ascending time
let files = fs.readdirSync(dataFolder).filter(i => i.endsWith('.jpg')).sort();

// Go through files, getting the metadata
let csvResult = "shortcode;url;likes;comments;tags;\n"; // init buffer
let count = 0;
let skipped = 0;

// Process each image, getting the tags through Vision Cognitive Service, and checking the json
// Note: Out of order will appear here!
const start = async () => {
    // Go through promises, but throttled
    const limit = 10; // 10 at a time

    // files = files.slice(0, 50);

    while (files.length > 0) {
        console.log(`[Progress] Processing ${limit} of the ${files.length} images`);
        let currentFiles = files.slice(0, limit);
        files = files.slice(limit, files.length);

         // Prepare promises
         let promises = [];
         currentFiles.forEach(i => {
            promises.push(new Promise(async (resolve, reject) => {
                try {
                    // Get tags
                    const imageBinary = await api.readImageByPath(dataFolder + i);
                    const res = await api.analyzeImage(imageBinary);
                    const tags = res.tags.map(t => t.name).join(",");
            
                    // Get JSON content
                    const file = require(`${dataFolder}${i.replace('.jpg', '.json')}`);
                    csvResult += `${file.shortcode};${file.display_url};${file.edge_media_preview_like.count};${file.edge_media_to_comment.count};${tags};\n`;
        
                    count++;
                } catch (e) {
                    skipped++;
                }
        
                // Update progress
                console.log(`[Progress] ${count + skipped}/${files.length}`);

                return resolve();
            }));
        })
    
        console.log('awaiting promises');
        await Promise.all(promises);
        console.log(`Sleeping for 1000ms`);
        await delay(1000);
    }

    // Write result
    fs.writeFileSync(outputFolder + 'result.csv', csvResult);

    console.log(`Processed ${count + skipped} files, success: ${count} skipped: ${skipped} and saved to ${outputFolder}result.csv`);
}

start();

