import React from 'react';
import { shallow } from 'enzyme';
import chai, { expect } from 'chai';
import chaiEnzyme from 'chai-enzyme';
import { StaticFile,__RewireAPI__ as StaticFileRewired } from '../../../src/components/data-types/static-file';

chai.use(chaiEnzyme());

describe('StaticFile', () => {
  it('should render column value as link when column value has url', () => {
    const props = {
      columnValue: {
        dataType: {
          name: "StaticFile",
          subType: ""
        },
        value:{
          "url": "https://te.azureedge.net/MMBI0001.jpg",
          "name": "MMBI0001.jpg",
          "id": "7fb8f298af07499c9de103f6cb72c6d3"
        }
      }
    };
    const wrapper = shallow(<StaticFile {...props}/>);
    expect(wrapper).to.have.descendants('a');
    expect(wrapper).to.have.prop('href','https://te.azureedge.net/MMBI0001.jpg?');
  });

  it('should render header and column value when column value is a string', () => {
    const props = {columnValue: {
      value: "MG0001",
      dataType: {
        name: "StaticFile",
        subType: ""
      }
    }};
    const wrapper = shallow(<StaticFile {...props}/>);
    expect(wrapper).to.not.have.descendants('a');
    expect(wrapper).to.have.text('MG0001');
  });

  it('should render input when it is editable and value is null',()=>{
    const props = {columnValue: {
      value: null,
      dataType: {
        name: "StaticFile",
        subType: ""
      },
      editable:true
    }};
    const wrapper = shallow(<StaticFile {...props}/>);
    expect(wrapper).to.have.descendants('button');
  });

  it('should render remove button if props do not have hide remove and value is present', () => {
    const props = {columnValue: {
      value: 1,
      dataType: {
        name: "StaticFile",
        subType: ""
      },
      editable:true
    }};
    window.fileList = [[{name:'MG0001'}],[{name:'MG0002'}]];
    StaticFileRewired.__Rewire__('styles', {delete: 'delete'});
    const wrapper = shallow(<StaticFile {...props}/>);
    expect(wrapper).to.include.text('MG0002');
    expect(wrapper).to.have.descendants('button');
    expect(wrapper.find('button').prop('className')).to.equal('delete');
  });

  it('should render remove button if props do not have hide remove and value is not present', () => {
    const props = {columnValue: {
      value: null,
      dataType: {
        name: "StaticFile",
        subType: ""
      },
      editable:true
    }};
    StaticFileRewired.__Rewire__('styles', {delete: 'delete'});
    const wrapper = shallow(<StaticFile {...props}/>);
    expect(wrapper).to.have.descendants('button');
    expect(wrapper.find('button').at(1).prop('className')).to.equal('delete');
  });

  it('should not render remove button if props have hide remove and value as null', () => {
    const props = {columnValue: {
      value: null,
      dataType: {
        name: "StaticFile",
        subType: ""
      },
      editable:true
    }, hideRemove:true};
    StaticFileRewired.__Rewire__('styles', {delete: 'delete'});
    const wrapper = shallow(<StaticFile {...props}/>);
    expect(wrapper).to.have.exactly(1).descendants('button');
  });

  it('should render link file and not render choose file button if props have column value and its url',() => {
    const props = {columnValue: {
      value: {name:"MG0001",url:"tw.com/MG001"},
      dataType: {
        name: "StaticFile",
        subType: ""
      },
      editable:true
    }, hideRemove:true};
    const wrapper = shallow(<StaticFile {...props}/>);
    expect(wrapper).to.have.descendants('a');
    expect(wrapper.find('a')).to.have.prop('href','tw.com/MG001?');
    expect(wrapper).to.not.have.descendants('button');
  });

  it('should not render link and render choose file button file if props have column value and its url',() => {
    const props = {columnValue: {
      value: null,
      dataType: {
        name: "StaticFile",
        subType: ""
      },
      editable:true
    }, hideRemove:true};
    const wrapper = shallow(<StaticFile {...props}/>);
    expect(wrapper).to.not.have.descendants('a');
    expect(wrapper).to.have.descendants('button');
  });
});
