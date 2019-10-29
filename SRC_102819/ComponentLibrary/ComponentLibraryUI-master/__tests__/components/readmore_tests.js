import chai, { expect } from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { ReadMore, __RewireAPI__ as ReadMoreRewired } from '../../src/components/read-more';

chai.use(chaiEnzyme());

describe('ReadMore', () => {
  describe('Render', () => {
    it('should render text with show more when text length exceeds it\'s props length', () => {
      const value = "Plywood General Purpose MR Grade 4mm AA";
      const props = {length: 10};
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      expect(wrapper.find('span')).to.have.text('Plywood Ge');
      expect(wrapper.find('div')).to.have.text('Plywood Ge Show More');
    });

    it('should render text with show less when text length exceeds it\'s props length and it\'s state is expanded', () => {
      const value = "Plywood General Purpose MR Grade 4mm AA";
      const props = {length: 10};
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      wrapper.find('a').simulate('click');
      expect(wrapper.find('span')).to.have.text('Plywood General Purpose MR Grade 4mm AA');
      expect(wrapper.find('div')).to.have.text('Plywood General Purpose MR Grade 4mm AA Show Less');
      wrapper.find('a').simulate('click');
      expect(wrapper.find('div')).to.have.text('Plywood Ge Show More');
    });

    it('should render text without show more toggle when text length does not exceed props length', () => {
      const value = "Plywood";
      const props = {length: 10};
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      expect(wrapper.find('span')).to.have.text('Plywood');
      expect(wrapper.find('div')).to.have.text('Plywood');
    });

    it('should render 2 elements of an array when given an array and no length', () => {
      const value = ["1","2","3"];
      const props = {};
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      expect(wrapper.find('span')).to.have.text('12');
      expect(wrapper.find('div')).to.have.text('12 Show More');
    });

    it('should render required number of elements of an array when given an array and length as property', () => {
      const value = ["1","2","3"];
      const props = {length: 2};
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      expect(wrapper.find('span')).to.have.text('12');
      expect(wrapper.find('div')).to.have.text('12 Show More');
    });

    it('should render all elements of an array when clicks show more toggle', () => {
      const value = ["1","2","3"];
      const props = {length: 2};
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      expect(wrapper.find('div')).to.have.text('12 Show More');
      wrapper.find('a').simulate('click');
      expect(wrapper.find('span')).to.have.text('123');
      expect(wrapper.find('div')).to.have.text('123 Show Less');
    });

    it('should render all elements of an array when length of the array is less than props length', () => {
      const value = ["1","2","3"];
      const props = {length: 4};
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      expect(wrapper.find('div')).to.have.text('123');
    });

    it('should return anchor tag which has class name link when length of the array is less than props length', () => {
      const value = ["1","2","3"];
      const props = {length: 2};
      ReadMoreRewired.__Rewire__('styles', {link: 'link'});
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      expect(wrapper.find('a').prop('className')).to.equal('link');
    });

    it('should have state contains expanded as false', () => {
      const value = ["1","2","3"];
      const props = {length: 4};
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      expect(wrapper).to.have.state('expanded',false);
    });

    it('should have state contains expanded as true when clicks show more toggle', () => {
      const value = ["1","2","3"];
      const props = {length: 2};
      const wrapper = shallow(<ReadMore {...props}>{value}</ReadMore>);
      expect(wrapper).to.have.state('expanded',false);
      wrapper.find('a').simulate('click');
      expect(wrapper).to.have.state('expanded',true);
    });
  });
});
