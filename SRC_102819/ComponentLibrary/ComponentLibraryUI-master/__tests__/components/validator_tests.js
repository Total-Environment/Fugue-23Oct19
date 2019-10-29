import chai, {expect} from 'chai';
import {validate } from '../../src/validator';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('Validator', ()=>{
  let columnDefinition;
  beforeEach(() => {
    columnDefinition = {dataType:{name: 'Int', subType: null}};
  });
  it('should return true if null is passed for any datatype', () => {
    expect(validate(null, columnDefinition).isValid).to.be.true;
  });

  it('should return true if value is NA for any datatype', () => {
    expect(validate('-- NA --', columnDefinition).isValid).to.be.true;
  });

  describe('String', () => {
    it('should return true if a valid string is passed', ()=>{
      columnDefinition = {dataType:{name: 'String', subType: null}};
      const value = 'This is a valid string';
      expect(validate(value, columnDefinition)).deep.equal( { isValid: true, msg: ''});
    });
  });

  describe('Range', () => {
    beforeEach(() => {
      columnDefinition = {dataType:{ name: 'Range', subType:null}};
    });
    it('should return true if a from and to are passed', ()=>{
      const value = {from: 12, to: 12};
      expect(validate(value, columnDefinition)).deep.equal({ isValid: true, msg: ''});
    });
    it('should return true if to is null', () => {
      const value = {from: 12, to: null};
      expect(validate(value, columnDefinition)).deep.equal({ isValid: true, msg: ''});
    });
    it('should return false if from is not a number', () => {
      const value = {from: '12.3.3', to: null};
      expect(validate(value, columnDefinition)).deep.equal({ isValid: false, msg: 'Not a valid number'});
    });
    it('should return false if to is not a number', () => {
      const value = {from: '12.3.3', to: null};
      expect(validate(value, columnDefinition)).deep.equal({ isValid: false, msg: 'Not a valid number'});
    });

    it('should return false if to is not a number', () => {
      const value = {from: '12.3.3', to: null};
      expect(validate(value, columnDefinition)).deep.equal({ isValid: false, msg: 'Not a valid number'});
    });


    context('When filterable is true', () => {
      it('should return false when given not valid decimal', () => {
        columnDefinition = {dataType:{ name: 'Range', subType:null},filterable:true};
        const value = '12.3.3';
        expect(validate(value, columnDefinition)).deep.equal({ isValid: false, msg: 'Not a valid number'});
      });

      it('should return true when given a valid decimal', () => {
        columnDefinition = {dataType:{ name: 'Range', subType:null},filterable:true};
        const value = '12.3';
        expect(validate(value, columnDefinition)).deep.equal({ isValid: true, msg: ''});
      });
    });

  });
  describe('Unit', () => {
    it('should return true if a valid Unit type is passed', ()=>{
      const value = {value: 3, type: 'mm'};
      const columnDefinition = {dataType:{name: 'Unit', subType: 'mm'}};

      expect(validate(value, columnDefinition).isValid).to.be.true;
    });

    it('should return false if invalid unit type is passed', () => {
      const value = {value: '3.3.3', type: 'mm'};
      const columnDefinition = { dataType : { name: 'Unit', subType: 'mm'}};

      expect(validate(value, columnDefinition)).deep.equal({isValid: false, msg:'Not a valid number'});
    });
  });

  describe('Boolean', () => {
    it('should return true if a valid Boolean type is passed', ()=>{
      const value = true;
      const columnDefinition = {dataType: { name: 'Boolean', subType:null}};

      expect(validate(value, columnDefinition).isValid).to.be.true;
    });
  });

  describe('MasterData', () => {
    it('should return true if a valid MasterData type is passed', () => {
      const value = 'Any Value';
      const columnDefinition = { dataType : {name: 'MasterData', subType: null}};

      expect(validate(value, columnDefinition)).deep.equal({isValid: true, msg: ''});
    });
  });

  describe('Default', () => {
    it('should return true if datatype does not exist', () => {
      expect(validate('sattar', {dataType:{name: 'Sattar'}})).deep.equal({isValid: true, msg: ''});
    });
  });

  describe('Money', () => {
    beforeEach(() => {
      columnDefinition = {dataType : { name: 'Money', subType: null}};
    });
    it('should return true if value is valid', () => {
      expect(validate({amount: '123', currency: 'INR'}, columnDefinition).isValid).to.be.true;
    });
    it('should return false if value not money', () => {
      expect(validate({amount: '123.3.3', currency: 'INR'}, columnDefinition)).deep.equal({isValid: false, msg: 'Not a valid number'});
    });

    it('should return false if currency is empty', () => {
      expect(validate({amount: '123.3', currency: ''}, columnDefinition)).deep.equal({isValid: false, msg: 'Currency is required'});
    });

    it('should return true if currency is empty but there is no amount', () => {
      expect(validate({amount: '', currency: ''}, columnDefinition).isValid).to.be.true;
    });
  });

  describe('Int', () => {
    beforeEach(() => {
      columnDefinition = {dataType : { name: 'Int', subType: null}};
    });
    it('should return true if value is int', () => {
      expect(validate('123', columnDefinition).isValid).to.be.true;
    });
    it('should return false if value not int', () => {
      expect(validate('abc', columnDefinition)).deep.equal({isValid: false, msg: 'Not a valid integer'});
    });
  });

  describe('Decimal', () => {
    beforeEach(() => {
      columnDefinition = {dataType : { name: 'Decimal', subType: null}};
    });
    it('should return true if value is int', () => {
      expect(validate('123', columnDefinition).isValid).to.be.true;
    });
    it('should return true if value is decimal', () => {
      expect(validate('123.3', columnDefinition).isValid).to.be.true;
    });
    it('should return false if value not decimal', () => {
      expect(validate('abc', columnDefinition)).deep.equal({isValid: false, msg: 'Not a valid number'});
    });
  });

  describe('Array', () => {
    beforeEach(() => {
      columnDefinition = {dataType : { name: 'Array', subType: {name: 'Money', subType: ''}}};
    });
    it('should return true if all the values are valid', () => {
      expect(validate([{amount: '123', currency: 'INR'}, {amount: '160000', currency: 'INR'}], columnDefinition).isValid).to.be.true;
    });
    it('should return false if any of the values is invalid', () => {
      expect(validate([{amount: '12.33.3.3', currency: 'INR'}, {amount: '160000', currency: 'INR'}], columnDefinition).isValid).to.be.false;
    });
    it('should return false if subType is static file and column is image which is in invalid format', () => {
      window.fileList=[[{name:'image.xlsx'}],[{name:'image.jpg'}]];
      columnDefinition = {dataType : { name: 'Array', subType: {name: 'StaticFile', subType: ''}}};
      expect(validate([0, 1], columnDefinition,'image').isValid).to.be.false;
    });
    it('should return true if subType is static file and column is an image which is in valid format', () => {
      window.fileList=[[{name:'1.png'}],[{name:'image.jpg'}]];
      columnDefinition = {dataType : { name: 'Array', subType: {name: 'StaticFile', subType: ''}}};
      expect(validate([0,1], columnDefinition,'image').isValid).to.be.true;
    });
    it('should return false if subType is static file and column is an image which are duplicates', () => {
      window.fileList=[[{name:'1.png'}],[{name:'image.jpg'}]];
      columnDefinition = {dataType : { name: 'Array', subType: {name: 'StaticFile', subType: ''}}};
      expect(validate([0,1,{name:'test.png'},{name:'test.png'}], columnDefinition,'image').isValid).to.be.false;
    });
    it('should return true if it contains filterable as true and sub type string which is valid', () => {
      columnDefinition = {dataType : { name: 'Array', subType: {name: 'String', subType: ''}},filterable:true};
      expect(validate('abcd', columnDefinition,'Governing Standard').isValid).to.be.true;
    });

    it('should return false if it contains filterable as true and sub type int which is invalid', () => {
      columnDefinition = {dataType : { name: 'Array', subType: {name: 'Int', subType: ''}},filterable:true};
      expect(validate('abcd', columnDefinition,'Governing Standard').isValid).to.be.false;
    });
  });

  describe('StaticFile',() => {
    beforeEach(() => {
      columnDefinition = {dataType: {name: 'StaticFile', subType: null}};
    });
    it('should return true if columnName is not given', () => {
      expect(validate(0, columnDefinition).isValid).to.be.true;
    });

    it('should return false if columnName image is not in valid format', () => {
      window.fileList=[[{name:'image.xlsx'}],[{name:'image.jpg'}]];
      expect(validate(0, columnDefinition,'image').isValid).to.be.false;
    });

    it('should return true if columnName image is in valid format', () => {
      // sinon.stub(window, 'fetchList').returns([[{name:'image.xlsx'}],[{name:'image.jpg'}]]);
      // global.window = {fetchList:[[{name:'image.xlsx'}],[{name:'image.jpg'}]]};
      window.fileList=[[{name:'image.xlsx'}],[{name:'image.jpg'}]];
      expect(validate(1, columnDefinition,'image').isValid).to.be.true ;
    });
  });
});
