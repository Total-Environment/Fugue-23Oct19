import moment from 'moment';
import R from 'ramda';
import axios from 'axios';
import {logError} from "./sumologic-logger";

export const apiUrl = (fragment) => `${window.API_URL || ''}/${fragment}`;

export const idFor = (...args) => args.join(' ').toLowerCase()
  .replace(/\s+/g, '-')           // Replace spaces with -
  .replace(/[^\w\-]+/g, '')       // Remove all non-word chars
  .replace(/\-\-+/g, '-')         // Replace multiple - with single -
  .replace(/^-+/, '')             // Trim - from start of text
  .replace(/-+$/, '');

export const tomorrowInIST = (date) => {
  return moment(date).add(1, 'day').tz('Asia/Calcutta').format('YYYY-MM-DD');
};

export const toIST = (date, format = 'YYYY-MM-DD') => {
  return moment(date).tz('Asia/Calcutta').format(format);
};

export const dropKeys = (obj, keys) => {
  const clone = Object.assign({}, obj);
  keys.forEach(key => delete clone[key]);
  return clone;
};

export const preventClickPropagation = (e) => {
  if (e) {
    if (e.stopPropagation) { e.stopPropagation() }
    if (e.nativeEvent && e.nativeEvent.stopImmediatePropagation) {
      e.nativeEvent.stopImmediatePropagation()
    }
  }
};


export const col = (details, column, ...args) => {
  if (!details) return;
  if (!column) return details;
  const columnDetails = details.columns.find(c => c.key === column);
  if (args.length === 0) return columnDetails;
  return R.path(args, columnDetails);
};

export const updateColumn = (details, headerKey, columnKey, newValue) => {
  let headers = details.headers
    .reduce((headerObj, header) => {
        let columns = header.columns;
        columns = header.columns.reduce((columnObj, column) => {
            if (header.key === headerKey && column.key === columnKey) {
              columnObj.push(Object.assign({}, column, newValue));
            }
            else {
              columnObj.push(column);
            }
            return columnObj;
          },
          []);
        headerObj.push({columns: columns, key: header.key, name: header.name});
        return headerObj;
      },
      []);
  return {headers};
};

export const head = (details, header, column, ...args) => {
  if (!details) return;
  if (!header) return details;
  if(!details.headers) return;
  const headerDetails = details.headers.find(h => h.key === header);
  if (!column) return headerDetails;
  return col(headerDetails, column, ...args);
};

export const getHeader = (details, headerKey) => {
  if(!details) return;
  if(!headerKey) return details;
  if(!details.headers) return;
  return details.headers.find(h => h.key === headerKey);
};

export const uploadStaticFiles = async (details) => {
  let arrayOfStaticFiles = [],
    staticFiles = [],
    failedArrayOfStaticFiles = [],
    listOfFailedStaticFiles = [],
    brandArrayStaticFiles = [],
    brandStaticFiles = [];
  let jsonToPost = {headers: []};
  details.headers
    .forEach(async (header) => {
      let columns = [];
      header.columns
        .forEach(async (column) => {
          column.value === '' ?
            columns.push({ value: null, key: column.key, name: column.name }) :
            columns.push({
              value: column.value,
              key: column.key,
              name: column.name
            });
          if (column.dataType.name === "StaticFile" &&
            column.value !== null) {
            if (typeof (column.value) === "number") {
              const file = window.fileList[column.value][0];
              const formData = new FormData();
              formData.append('File', file);
              staticFiles.push({
                headerKey:header.key, columnKey:column.key, formData, columnName: column.name
              });
            }
          }
          if (column.dataType.name === 'Array'
            && column.dataType.subType.name === 'Brand') {
            let brands = column.value;
            brands.forEach((brand, index) => {
              brand
                .columns
                .filter(x => (x.dataType.name === 'StaticFile' && R.is(Number, x.value))
                || (x.dataType.name === 'Array'
                  && (x.dataType.subType && x.dataType.subType.name) === 'StaticFile'))
                .forEach(y => {
                  if (y.dataType.name === 'Array' && y.dataType.subType.name === 'StaticFile') {
                    let files = (y.value || [])
                      .map(fileId => R.is(Number, fileId) ? window.fileList[fileId][0] : null)
                      .filter(c => c);
                    const formData = new FormData();
                    files.forEach(file => formData.append('File', file));
                    if (files && files.length) {
                      brandStaticFiles.push({
                        headerKey:header.key, columnKey:column.key, brandFieldKey: y.key, formData, index, columnName: column.name
                      });
                    }
                    return;
                  }
                  const file = window.fileList[y.value][0];
                  if (file) {
                    const formData = new FormData();
                    formData.append('File', file);
                    brandStaticFiles.push({
                      headerKey:header.key, columnKey:column.key,
                      brandFieldKey: y.key,
                      type: y.dataType.name,
                      formData,
                      index,
                      columnName: column.name
                    });
                  }
                });
            });
          }
          if (column.dataType.name === "Array" &&
            column.dataType.subType.name === "StaticFile" &&
            R.isArrayLike(column.value)) {
            const length = column.value.length;
            for (let i = 0; i < length; i++) {
              if (typeof (column.value[i]) === "number") {
                const file = window.fileList[column.value[i]][0];
                const formData = new FormData();
                formData.append('File', file);
                arrayOfStaticFiles.push({
                  headerKey:header.key, columnKey:column.key, formData, index: i, columnName: column.name
                });
              }
            }
          }
        });
      jsonToPost.headers.push({columns,key: header.key,name: header.name});
    });

  await Promise.all(arrayOfStaticFiles
    .map(async (file) => {
      try {
        const response = await axios.post(apiUrl(`upload/${containerName}`), file.formData);
        let value = head(jsonToPost, file.headerKey, file.columnKey, 'value');
        value[file.index] = response.data[0];
        jsonToPost = updateColumn(jsonToPost,file.headerKey,file.columnKey,{value : value});
      }
      catch (error) {
        logException(error);
        failedArrayOfStaticFiles
          .push({ headerKey: file.headerKey, index: file.index, columnKey: file.columnKey,columnName: file.columnName });
        listOfFailedStaticFiles.push(file.columnName);
      }
    }));


  for (let i = 0; i < failedArrayOfStaticFiles.length; i++) {
    const staticFile = failedArrayOfStaticFiles[i];
    if (staticFile.index > -1 && head(jsonToPost,staticFile.headerKey,staticFile.columnKey,'value')) {
      let value = head(jsonToPost, staticFile.headerKey, staticFile.columnKey, 'value');
      value.splice(staticFile.index, 1);
      if (value.length === 0) {
        jsonToPost = updateColumn(jsonToPost,staticFile.headerKey,staticFile.columnKey,{value : null});
      }
    }
  }

  await Promise.all(staticFiles
    .map(async (file) => {
      try {
        const response = await axios.post(apiUrl(`upload/${containerName}`), file.formData);
        jsonToPost = updateColumn(jsonToPost,file.headerKey,file.columnKey,{value : response.data[0]});
      }
      catch (error) {
        logException(error);
        jsonToPost = updateColumn(jsonToPost,file.headerKey,file.columnKey,{value : null});
        listOfFailedStaticFiles.push(file.columnName);
      }
    }));

    await Promise.all(brandStaticFiles.map(async file => {
      try {
        const response = await axios.post(apiUrl(`upload/${containerName}`), file.formData);
        let brandColumns = head(jsonToPost,file.headerKey,file.columnKey,'value')[file.index].columns;
        brandColumns = brandColumns.map(x => {
          if (x.key === file.brandFieldKey) {
            return Object.assign({}, x, {
              value: x.dataType.name === 'StaticFile' ? response.data[0] : (x.value || []).
                filter(x => !R.is(Number, x)).concat(response.data)
            });
          }
          return x;
        });
        let value = head(jsonToPost, file.headerKey, file.columnKey, 'value');
        value[file.index].columns = brandColumns;
        jsonToPost = updateColumn(jsonToPost,file.headerKey,file.columnKey,{value:  value});
      }
      catch (err) {
        logException(error);
        let brandColumns = head(jsonToPost,file.headerKey,file.columnKey,'value')[file.index].columns;
        brandColumns = brandColumns.map(x => {
          if (x.key === file.brandFieldKey) {
            return Object.assign({}, x, {
              value: null
            });
          }
          return x;
        });
        let value = head(jsonToPost,file.headerKey,file.columnKey,'value')[file.index].columns;
        value[file.index].columns = brandColumns;
        jsonToPost = updateColumn(jsonToPost,file.headerKey,file.columnKey,{value: value});
        listOfFailedStaticFiles.push(file.columnName);
      }
    }));

  return ({ jsonToPost, listOfFailedStaticFiles });
};

export function updateClassificationDetails(columns, children) {
  const currentColumn = columns[0];
  if (!currentColumn) return columns;
  currentColumn.dataType.values = {
    values: children.map(child => child.name).filter(name => name !== "null"),
    status: 'fetched'
  };
  if (!currentColumn.value) {
    columns.filter((c, i) => i !== 0).forEach(column => {
      column.value = '';
      if (column.dataType.values) {
        column.dataType.values.values = [];
      }
    });
    return columns;
  }
  const remainingDependencyDefinition = children.find(child => child.name === currentColumn.value);
  if (!remainingDependencyDefinition) {
    columns.filter((c, i) => i !== 0).forEach(column => {
      column.value = '';
      if (column.dataType.values) {
        column.dataType.values.values = [];
      }
    });
    return columns;
  }
  return R.prepend(currentColumn, updateClassificationDetails(columns
    .filter(columnName => columnName.name !== "key" && columnName.name !== "name")
    .filter((c, i) => i !== 0), remainingDependencyDefinition.children));
}

export const isFetched = (obj) => !!obj && !obj.isFetching && !obj.error;
export const isFetchedAll = (...objs) => R.all(isFetched, objs);
export const isRatesFetched = (obj) => !!obj && !obj.isFetchingRates;
export const needsFetching = (obj) => !obj || (!obj.values && !obj.isFetching);
export const isFetchedDeprecated = (obj) => !!obj && obj.status === 'fetched';
export const updateIn = (obj, result, ...path) => R.set(Array.isArray(path) ? R.lensPath(path) : R.lensProp(path), result, obj);

export const logException = (ex, context) => {
  logError(ex);
  window.console && console.error && console.error(ex);
};

export const isFeatureEnabled = (feature) => !window.DISABLED_FEATURES[feature];

export const sasContainer = {
  tokenHash: {},
};

export const getSaasToken = (container) => {
  const searchParams = sasContainer.tokenHash[container];
  return (searchParams && searchParams.toString()) || '';
};

export const containerName = 'static-files';

// export const paginationConnector = (connector) =>
//   (dispatch, ownProps) => Object.assign({}, connector(dispatch, ownProps), {onAmendQueryParams: x => x});
