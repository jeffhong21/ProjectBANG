#target photoshop;

while(app.activeDocument.layerSets.length){

activeDocument.activeLayer = app.activeDocument.layerSets[0];

executeAction(stringIDToTypeID("newPlacedLayer"), new ActionDescriptor(), DialogModes.NO);

activeDocument.activeLayer.rasterize(RasterizeType.ENTIRELAYER);

}