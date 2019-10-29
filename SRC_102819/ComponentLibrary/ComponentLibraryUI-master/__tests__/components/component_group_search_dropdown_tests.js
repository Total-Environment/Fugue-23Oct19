import React from 'react';
import {shallow} from 'enzyme';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';

import {ComponentGroupDropdown} from '../../src/components/component-group-search-dropdown'

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe("ComponentGroupDropDown", () => {
  let props, wrapper;
  context("when search result fetched successfully", () => {
    beforeEach(() => {
      props = {
        componentType: "material",
        searchResponse: "we found keyword in",
        groups: ["CLAY Material", "Cement Material"]
      };
      wrapper = shallow(<ComponentGroupDropdown {...props}/>);
    });
    it("should display the search response", () => {
      expect(wrapper).to.have.descendants('div');
      expect(wrapper.find('div').first()).to.have.descendants('h4');
      expect(wrapper.find('div').first().find('h4')).to.have.text('we found keyword in');
    });
    it("should display the group as list element", () => {
      expect(wrapper).to.have.descendants('div');
      expect(wrapper).to.have.descendants('ul');
      expect(wrapper).to.have.descendants('li');
      expect(wrapper).to.have.descendants('Link');
    });
  });
});
