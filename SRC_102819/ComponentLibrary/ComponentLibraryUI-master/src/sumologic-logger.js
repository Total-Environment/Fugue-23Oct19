import SumoLogic from 'sumologic.js';

const sumoLogger = new SumoLogic(window.SUMOLOGIC_CONFIG);

export const logInfo = (message) => {
  sumoLogger.info(message);
};

export const logError = (message) => {
  sumoLogger.error(message);
};

export const logWarning = (message) => {
  sumoLogger.warn(message);
};
