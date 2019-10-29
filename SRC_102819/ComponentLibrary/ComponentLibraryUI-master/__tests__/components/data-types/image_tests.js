import React from 'react';
import { shallow } from 'enzyme';
import chai, { expect } from 'chai';
import chaiEnzyme from 'chai-enzyme';
import { Image } from '../../../src/components/data-types/image';

chai.use(chaiEnzyme());

describe('Image', () => {
  it('should render column value as link when column value has url', () => {
    const props = {columnName: "Image", columnValue: {
      value:
        {
          "url": "https://te-qa-cdn.azureedge.net/static-files/MMBI0001.jpg",
          "name": "MMBI0001.jpg",
          "id": "7fb8f298af07499c9de103f6cb72c6d3"
        }
    }};
    const wrapper = shallow(<Image {...props}/>);
    expect(wrapper).to.have.descendants('a');
    expect(wrapper).to.have.descendants('img');
    expect(wrapper).to.have.prop('href','https://te-qa-cdn.azureedge.net/static-files/MMBI0001.jpg?');
  });

  it('should render column value when column value does not have url', () => {
    const props = {columnName: "Image", columnValue: {
      value: "MG0001"
    }};
    const wrapper = shallow(<Image {...props}/>);
    expect(wrapper).to.not.have.descendants('a');
    expect(wrapper).to.not.have.descendants('img');
    expect(wrapper).to.have.text('MG0001');
  });
});
