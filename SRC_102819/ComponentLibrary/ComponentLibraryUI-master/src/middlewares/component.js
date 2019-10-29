import * as R from 'ramda';


const capsRegex = /[A-Z]+/g;
const transformAllCapsStr = R.compose(
  R.join(''),
  R.values,
  R.mapObjIndexed((v, i, o) => {
    if (+i === R.dec(R.length(o))) {
      return v;
    }
    return v.toLowerCase();
  }),
  R.split('')
);

function transformName(headerName) {
  if (R.isNil(headerName) || R.isEmpty(headerName)) {
    return '';
  }
  let splitStr = R.split(' ', headerName);
  let firstOriginalString = R.head(splitStr);
  let capsLockedString = R.head(R.match(capsRegex, firstOriginalString));

 let isAllCaps = capsLockedString === firstOriginalString;
 if (isAllCaps) {
    return R.join(' ',
      R.concat(
        [capsLockedString],
        R.drop(1, splitStr)));
  }
 return R.join(' ', R.concat([R.toLower(R.head(splitStr))], R.drop(1, splitStr)));
}

function transformDataToRequestData(headers,definition) {
  let resultHeaders = {"headers":[]};
  Object.keys(headers)
    .forEach(key =>
    {
      if(key === "Classification" && definition !== undefined) {
        definition.headers.reduce((headerObj,header) => {
          let result;
          if(header.name === "Classification") {
            result = header.columns.reduce((columnObj, column) => {
              columnObj.push(
                {value:headers[key][column.name] && headers[key][column.name].value,
                  key:column.key,
                  name:column.name});
              return columnObj;
            },
              []);
           resultHeaders.headers.push({"columns":result,"name":header.name,"key":header.key});
          }
        },{});
      }
      else
      {
        let headerKey = headers[key].key;
        let name = headers[key].name;
        let columns = [];
        Object.keys(headers[key])
          .filter(columnName => columnName !== "key" && columnName !== "name")
          .forEach(columnKey => {
            columns.push({
              "key": headers[key][columnKey].key, "name": columnKey, "value": headers[key][columnKey].value
            });
          });
        resultHeaders.headers.push({"columns": columns,"key":headerKey,"name":name});
      }
    });
  return resultHeaders;
}

function transformDataFromResponse(data) {
  let headers = data.headers || [];

  let result = {};

  R.forEach(header => {
    let headerName = header.name || '';
    let headerKey = header.key || '';
    if (R.isEmpty(headerName)) {
      return;
    }
    let materialHeaderName = transformName(headerName);
    let innerColumns = {};

    R.forEach(column => {
      let columnName = column.name || '';
      if (R.isEmpty(columnName)) {
        return;
      }
      let columnHeaderName = transformName(columnName);

      innerColumns = R.assoc(columnHeaderName, column, innerColumns);
      innerColumns["key"] = headerKey;
      innerColumns["name"] = headerName;
    }, header.columns || []);
    result = R.assoc(materialHeaderName, innerColumns, result);
  }, headers);
  return R.assoc('group', data.group, R.assoc('id', data.id || data.code, result));
}

export default function(data, options,definition) {
  if (!R.is(Object, data)) {
    return {};
  }
  let toRequest = (options && options.toRequest) || false;
  return toRequest ? transformDataToRequestData(data,definition) : transformDataFromResponse(data,definition);
};
