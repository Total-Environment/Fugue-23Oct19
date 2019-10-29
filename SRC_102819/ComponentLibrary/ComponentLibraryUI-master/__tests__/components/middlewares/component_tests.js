import componentDataTransformer from '../../../src/middlewares/component';
import chai, { expect } from 'chai';

describe('middleware component', () => {
  it('should return null for non-object params', () => {
    expect(componentDataTransformer(null)).to.eql({});
  });

  it('should return object with group and id when headers are empty', () => {
    expect(componentDataTransformer({headers:[]})).to.eql({group:undefined,id:undefined});
  });

  it('should return transformed object without header if header does not have name', () => {
    let data = {headers:[{columns:[],key:'key'}]};
    expect(componentDataTransformer(data)).to.eql({group:undefined,id:undefined});
  });

  it("should return transformed object without header if header's name is empty", () => {
    let data = {headers:[{columns:[],key:'key',name:''}]};
    expect(componentDataTransformer(data)).to.eql({group:undefined,id:undefined});
  });

  it('should return object without columns if column in header does not have name', () => {
    let data = {headers:[{columns:[{name:''}],key:'key',name:'Header'}]};
    expect(componentDataTransformer(data)).to.eql({ group:undefined,id:undefined, header:{}});
  });

  it('should return object when given header with columns',() => {
    let data = {headers:[{columns:[{name:'Material Level 1'}],key:'key',name:'Header'}]};
    expect(componentDataTransformer(data)).to.eql({ group:undefined,id:undefined, header:{'material Level 1':{ name: 'Material Level 1' },key:'key',name:'Header'}});
  });

  it('should return object contains same name when column name contains a string with all CAPS', () => {
    let data = {
      headers: [{
        columns: [{
          name: 'HSN Code',
          key: 'hsn_code',
        }],
        key: 'key',
        name: 'Header'
      }]
    };

    expect(componentDataTransformer(data)).to.eql({
      group: undefined,
      id: undefined,
      header: {
        'HSN Code': {
          name: 'HSN Code',
          key: 'hsn_code'
        },
        key: 'key',
        name: 'Header'
      }
    });
  });

 it('should return expected Object when given definition and option toRequest true', () => {
   let data = {header: {
     'hsN Code': {
       name: 'HSN Code',
       key: 'hsn_code',
       value:'value'
     },
     key: 'key',
     name: 'Header'
   }};
   console.log(JSON.stringify(componentDataTransformer(data,{toRequest:true})));
     expect(componentDataTransformer(data, {toRequest: true})).to.eql({
       headers: [{
         columns: [{
           name: 'hsN Code',
           key: 'hsn_code',
           value:'value'
         }],
         key: 'key',
         name: 'Header',
       }]
     });
 });

 it('should return expected Object when given definition and option toRequest true',() => {
   let data = {Classification: {
     'Material Level 1': {
       value:'value'
     }
   }};

   let definition = {headers:
     [{ "columns": [
      {
         "dataType": {
           "name": "Constant",
           "subType": "Primary"
         },
         "isRequired": false,
         "isSearchable": true,
         "key": "material_level_1",
         "name": "Material Level 1"
     }],
   "key": "classification",
   "name": "Classification"
     }]
   };
   expect(componentDataTransformer(data, {toRequest: true},definition)).to.eql({
     headers: [{
       columns: [{
         "key": "material_level_1",
         "name": "Material Level 1",
         value:'value'
       }],
       key: 'classification',
       name: 'Classification'
     }]
   });
 });
});
